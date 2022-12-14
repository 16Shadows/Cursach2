using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Content;
using DMOrganizerModel.Interface.NavigationTree;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserActivities;
using static System.Net.Mime.MediaTypeNames;

namespace DMOrganizerModel.Implementation.Content
{
    /// <summary>
    /// Implementation of ISection, shared by Section and Document
    /// </summary>
    internal abstract class SectionBase : ItemBase, ISection
    {
        #region Properties
        public string Content { get; protected set; }

        private ObservableList<ISection>? m_Children;
        protected ObservableList<ISection> Children
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
        public SectionBase(OrganizerModel organizer, string title, string content, int itemID) : base(organizer, title, itemID)
        {
            Content = content ?? throw new ArgumentNullException(nameof(title));
            m_Children = new ObservableList<ISection>();
            m_Sections = new Dictionary<string, Section>();
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
        }

        public Task UpdateContent(string text)
        {
            CheckDisposed();
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            return Task.Run(() =>
            {
                lock (SyncRoot)
                {
                    string oldContent = Content;
                    try
                    {
                        Content = text;
                        Organizer.UpdateSectionContent(this, text);
                    }
                    catch (Exception e)
                    {
                        Content = oldContent;
                        InvokeContentUpdated(OperationResultEventArgs.ErrorType.InternalError, e.Message);
                    }
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

            return Task.Run(() =>
            {
                if (!StorageModel.IsValidTitle(title))
                {
                    InvokeSectionCreated(OperationResultEventArgs.ErrorType.InvalidArgument, "Invalid title", title, null);
                    return;
                }
                lock (SyncRoot)
                {
                    Section? sec = null;
                    try
                    {
                        sec = Organizer.CreateSection(this, title);
                        Sections.Add(title, sec);
                        Children.Add(sec);
                        InvokeSectionCreated(OperationResultEventArgs.ErrorType.None, null, null, sec);
                    }
                    catch (Exception e)
                    {
                        sec?.Dispose();
                        InvokeSectionCreated(OperationResultEventArgs.ErrorType.InternalError, e.Message, title, null);
                    }
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

            return Task.Run(() =>
            {
                lock (SyncRoot)
                {
                    try
                    {
                        Sections.Remove(sectionInstance.Title);
                        Organizer.DeleteSection(sectionInstance);
                        Children.Remove(section);
                        sectionInstance.Dispose();
                    }
                    catch (Exception e)
                    {
                        InvokeSectionDeleted(section.Title, OperationResultEventArgs.ErrorType.InternalError, e.Message);
                    }
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
