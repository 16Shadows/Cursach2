using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Content;
using DMOrganizerModel.Interface.NavigationTree;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Windows.ApplicationModel.UserActivities;

namespace DMOrganizerModel.Implementation.Content
{
    /// <summary>
    /// Implementation of ISection, shared by Section and Document
    /// </summary>
    internal abstract class SectionBase : ItemBase, ISection
    {
        #region Properties
        private string m_Content;
        public string Content
        {
            get
            {
                CheckDisposed();
                return m_Content;
            }
            protected set
            {
                CheckDisposed();
                if (m_Content == value)
                    return;
                m_Content = value ?? throw new ArgumentNullException(nameof(Content));
                InvokePropertyChanged(nameof(Content));
            }
        }

        private SectionBase? m_Parent;
        public SectionBase? Parent
        {
            get
            {
                CheckDisposed();
                return m_Parent;
            }
            protected set
            {
                CheckDisposed();
                if (m_Parent == value)
                    return;
                m_Parent = value;
                InvokePropertyChanged(nameof(Parent));
            }
        }
        ISection ISection.Parent => Parent;

        private ObservableList<ISection>? m_Children;
        public ObservableList<ISection> Children
        {
            get
            {
                CheckDisposed();
                return m_Children;
            }
        } 
        IObservableReadOnlyList<ISection> ISection.Children => Children;

        private Dictionary<string, Section>? m_Sections;

        protected Dictionary<string, Section> Sections
        {
            get
            {
                CheckDisposed();
                return m_Sections;
            }
        }
        #endregion
        
        #region Events
        public event OperationResultEventHandler<ISection>? ContentUpdated;
        public event OperationResultEventHandler<ISection, SectionCreatedEventArgs>? SectionCreated;
        public event OperationResultEventHandler<ISection, SectionDeletedEventArgs>? SectionDeleted;
        #endregion

        #region Constructors
        public SectionBase(OrganizerModel organizer, SectionBase? parent, string title, string content, int itemID) : base(organizer, title, itemID)
        {
            m_Content = content ?? throw new ArgumentNullException(nameof(title));
            m_Children = new ObservableList<ISection>();
            m_Sections = new Dictionary<string, Section>(new NoCaseStringComparer());
            m_Parent = parent;
        }
        #endregion

        #region Interface
        protected override void CheckDisposed()
        {
            base.CheckDisposed();
            if (m_Sections == null || m_Children == null)
                throw new ObjectDisposedException(GetType().Name);
        }

        public override void Dispose()
        {
            base.Dispose();
            m_Children = null;

            foreach (var sec in m_Sections)
                sec.Value.Dispose();

            m_Sections = null;
            m_Content = null;
            m_Parent = null;
        }

        public Task UpdateContent(string text)
        {
            CheckDisposed();
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            return Task.Run(() =>
            {
                string oldContent = null;
                try
                {
                    lock (SyncRoot)
                        oldContent = Content;
                    Organizer.UpdateSectionContent(this, text);
                    dispatcher.BeginInvoke(() =>
                    {
                        lock (SyncRoot)
                            Content = text;
                        InvokeContentUpdated(OperationResultEventArgs.ErrorType.None, null);
                    });
                }
                catch (Exception e)
                {
                    if (oldContent != null && oldContent != Content)
                        Content = oldContent;
                    dispatcher.BeginInvoke(() => InvokeContentUpdated(OperationResultEventArgs.ErrorType.InternalError, e.ToString()));
                }
            }); 
        }

        protected void InvokeContentUpdated(OperationResultEventArgs.ErrorType error, string? errorText)
        {
            ContentUpdated?.Invoke(this, new OperationResultEventArgs
            {
                Error = error,
                ErrorText = errorText
            });
        }

        public Task CreateSection(string title)
        {
            CheckDisposed();
            if (title == null)
                throw new ArgumentNullException(nameof(title));

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            return Task.Run(() =>
            {
                if (!StorageModel.IsValidTitle(title))
                {
                    dispatcher.BeginInvoke(() => InvokeSectionCreated(OperationResultEventArgs.ErrorType.InvalidArgument, "Invalid title", title, null));
                    return;
                }
                Section? sec = null;
                try
                {
                    sec = Organizer.CreateSection(this, title);
                    lock (SyncRoot)
                        Sections.Add(title, sec);
                    
                    dispatcher.BeginInvoke(() =>
                    {
                        lock (SyncRoot)
                            Children.Add(sec);
                        InvokeSectionCreated(OperationResultEventArgs.ErrorType.None, null, null, sec);
                    });
                }
                catch (Exception e)
                {
                    sec?.Dispose();
                    dispatcher.BeginInvoke(() => InvokeSectionCreated(OperationResultEventArgs.ErrorType.InternalError, e.ToString(), title, null));
                }
            });
        }

        protected void InvokeSectionCreated(OperationResultEventArgs.ErrorType error, string? errorText, string? title, ISection? instance)
        {
            SectionCreated?.Invoke(this, new SectionCreatedEventArgs()
            {
                Error = error,
                ErrorText = errorText,
                Title = title,
                SectionInstance = instance
            });
        }

        public Task DeleteSection(ISection section)
        {
            CheckDisposed();
            if (section == null)
                throw new ArgumentNullException(nameof(section));

            if (section is not Section sectionInstance)
                throw new ArgumentException("Incompatiable instance", nameof(section));

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            return Task.Run(() =>
            {
                try
                {
                    Organizer.DeleteSection(sectionInstance);
                    lock (SyncRoot)
                        Sections.Remove(sectionInstance.Title);
                    dispatcher.BeginInvoke(() =>
                    {
                        lock (SyncRoot)
                            Children.Remove(section);
                        sectionInstance.Dispose();
                    });
                }
                catch (Exception e)
                {
                    dispatcher.BeginInvoke(() => InvokeSectionDeleted(section.Title, OperationResultEventArgs.ErrorType.InternalError, e.ToString()));
                }
            });
        }

        protected void InvokeSectionDeleted(string title, OperationResultEventArgs.ErrorType error, string? errorText)
        {
            SectionDeleted?.Invoke(this, new SectionDeletedEventArgs(title)
            {
                Error = error,
                ErrorText = errorText
            });
        }
        #endregion

        #region Methods
        public void AddSection(Section section)
        {
            CheckDisposed();
            if (m_Sections.ContainsKey(section.Title))
                throw new ArgumentException("Section with the same title is already present", nameof(section));

            Sections.Add(section.Title, section);
            Children.Add(section);
        }

        public Section? GetSection(string title)
        {
            CheckDisposed();
            m_Sections.TryGetValue(title, out Section? result);
            return result;
        }
        #endregion
    }
}
