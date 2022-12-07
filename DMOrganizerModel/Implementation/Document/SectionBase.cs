using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Document;
using DMOrganizerModel.Interface.NavigationTree;
using System;
using System.Text;

namespace DMOrganizerModel.Implementation.Document
{
    /// <summary>
    /// Implementation of ISection, shared by Section and Document
    /// </summary>
    internal abstract class SectionBase : ISection
    {
        #region Properties
        public string Title { get; protected set; }
        public string Content { get; protected set; }
        public IObservableList<ISection> Children { get; }
        #endregion

        #region Events
        public event OperationResultEventHandler<INavigationTreeNodeBase>? Renamed;
        public event OperationResultEventHandler<INavigationTreeNodeBase>? ContentUpdated;
        public event OperationResultEventHandler<ISection>? ChildDeleted;
        #endregion

        #region Constructors
        public SectionBase(string title, string content)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Content = content ?? throw new ArgumentNullException(nameof(title));
            Children = new SectionChildrenCollection();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Is used to efficently construct path to this instance
        /// Specific implementation depends on the object (Section or Document)
        /// </summary>
        /// <param name="len">The number of characters required by previous steps of the algorithm to construct full path</param>
        /// <returns></returns>
        public abstract StringBuilder GetPath(int len = 0);

        public abstract bool Rename(string name);

        public abstract bool UpdateContent(string text);

        public bool DeleteChild(ISection section)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
