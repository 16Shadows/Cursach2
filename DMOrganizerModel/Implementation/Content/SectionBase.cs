using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Content;
using DMOrganizerModel.Interface.NavigationTree;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        #endregion

        #region Fields
        private Dictionary<string, SectionBase>? m_Sections;
        protected Dictionary<string, SectionBase> Sections
        {
            get
            {
                CheckDisposed();
                return m_Sections;
            }
        }
        #endregion

        #region Events
        public event OperationResultEventHandler<INavigationTreeNodeBase>? ContentUpdated;
        public event OperationResultEventHandler<ISection>? ChildDeleted;
        public event OperationResultEventHandler<INavigationTreeNodeBase>? SectionCreated;
        #endregion

        #region Constructors
        public SectionBase(OrganizerModel organizer, string title, string content, int itemID) : base(organizer, title, itemID)
        {
            Content = content ?? throw new ArgumentNullException(nameof(title));
            m_Children = new ObservableList<ISection>();
            m_Sections = new Dictionary<string, SectionBase>();
        }
        #endregion

        #region Methods
        public void AddSection(SectionBase section)
        {
            CheckDisposed();
            if (m_Sections.ContainsKey(section.Title))
                throw new ArgumentException("Section with the same title is already present", nameof(section));

            Sections.Add(section.Title, section);
            Children.Add(section);
        }

        public void RemoveSection(SectionBase section)
        {
            CheckDisposed();
            if (!m_Sections.ContainsKey(section.Title))
                throw new ArgumentException("The section is not present in this section", nameof(section));

            Sections.Add(section.Title, section);
            Children.Add(section);
        }


        public SectionBase? GetSection(string title)
        {
            CheckDisposed();
            m_Sections.TryGetValue(title, out SectionBase? result);
            return result;
        }

        public abstract bool UpdateContent(string text);

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
            m_Sections = null;
        }

        Task ISection.UpdateContent(string text)
        {
            throw new NotImplementedException();
        }

        public Task CreateSection(string title)
        {
            throw new NotImplementedException();
        }

        public Task DeleteSection(ISection section)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
