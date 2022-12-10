using System;
using System.Text;

using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Content;
using DMOrganizerModel.Implementation.NavigationTree;
using DMOrganizerModel.Implementation.Model;

namespace DMOrganizerModel.Implementation.Content
{
    internal class Document : SectionBase, IDocument
    {
        #region Properties
        public IObservableCollection<string> Tags { get; }
        #endregion

        #region Fields
        protected NavigationTreeDocument m_NavigationTreeInstance;
        #endregion

        #region Constructors
        public Document(OrganizerModel organizer, NavigationTreeDocument parent, string title, string content) : base(organizer, title, content)
        {
            m_NavigationTreeInstance = parent ?? throw new ArgumentNullException(nameof(parent));
            Tags = new ObservableList<string>();
            Tags.CollectionChanged += Tags_CollectionChanged;
        }
        #endregion

        #region EventHandlers
        private void Tags_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
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
