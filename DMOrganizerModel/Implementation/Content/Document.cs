using System;
using System.Text;

using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Content;
using DMOrganizerModel.Implementation.NavigationTree;
using DMOrganizerModel.Implementation.Model;
using System.Threading.Tasks;
using DMOrganizerModel.Interface.NavigationTree;

namespace DMOrganizerModel.Implementation.Content
{
    internal sealed class Document : SectionBase, IDocument
    {
        #region Properties
        public ObservableList<string> Tags { get; }
        IObservableReadOnlyCollection<string> IDocument.Tags => Tags;
        #endregion

        #region Fields
        private NavigationTreeDocument m_NavigationTreeInstance;
        #endregion

        #region Constructors
        public Document(OrganizerModel organizer, NavigationTreeDocument treeNode, string title, string content, int itemID) : base(organizer, title, content, itemID)
        {
            m_NavigationTreeInstance = treeNode ?? throw new ArgumentNullException(nameof(treeNode));
            Tags = new ObservableList<string>();
        }
        #endregion

        #region Methods
        public override StringBuilder GetPath(int len = 0)
        {
            return m_NavigationTreeInstance.GetPath(len);
        }

        public override bool Rename(string name)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateContent(string text)
        {
            throw new NotImplementedException();
        }

        public Task AddTag(string tag)
        {
            throw new NotImplementedException();
        }

        public Task RemoveTag(string tag)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
