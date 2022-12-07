using System;

using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Document;

using DMOrganizerModel.Implementation.NavigationTree;
using System.Text;

namespace DMOrganizerModel.Implementation.Document
{
    internal class Document : SectionBase, IDocument
    {
        #region Properties
        public IObservableCollection<string> Tags => throw new NotImplementedException();
        #endregion

        #region Fields
        protected NavigationTreeDocument m_NavigationTreeInstance;
        #endregion

        #region Constructors
        public Document(NavigationTreeDocument parent, string title, string content) : base(title, content)
        {
            m_NavigationTreeInstance = parent ?? throw new ArgumentNullException(nameof(parent));
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
        #endregion
    }
}
