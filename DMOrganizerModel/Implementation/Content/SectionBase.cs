using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Content;
using DMOrganizerModel.Interface.NavigationTree;
using System;
using System.Text;

namespace DMOrganizerModel.Implementation.Content
{
    /// <summary>
    /// Implementation of ISection, shared by Section and Document
    /// </summary>
    internal abstract class SectionBase : ItemBase, ISection
    {
        #region Properties
        public string Title { get; protected set; }
        public string Content { get; protected set; }

        public IObservableReadOnlyList<ISection> Children { get; }
        #endregion

        #region Events
        public event OperationResultEventHandler<INavigationTreeNodeBase>? Renamed;
        public event OperationResultEventHandler<INavigationTreeNodeBase>? ContentUpdated;
        public event OperationResultEventHandler<ISection>? ChildDeleted;
        #endregion

        #region Constructors
        public SectionBase(OrganizerModel organizer, string title, string content) : base(organizer, title)
        {
            Content = content ?? throw new ArgumentNullException(nameof(title));
            Children = new ObservableList<ISection>();
        }
        #endregion

        #region Methods
        public SectionBase GetSection(string title)
        {
            throw new NotImplementedException();
        }

        public abstract bool UpdateContent(string text);
        #endregion
    }
}
