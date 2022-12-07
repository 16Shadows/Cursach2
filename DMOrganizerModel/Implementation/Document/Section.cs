using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Document;
using DMOrganizerModel.Interface.NavigationTree;
using System;

namespace DMOrganizerModel.Implementation.Document
{
    internal class Section : ISection
    {
        public string Title => throw new NotImplementedException();

        public string Content => throw new NotImplementedException();

        public IObservableList<ISection> Children => throw new NotImplementedException();

        public event OperationResultEventHandler<INavigationTreeNodeBase>? Renamed;
        public event OperationResultEventHandler<INavigationTreeNodeBase>? ContentUpdated;

        public bool Rename(string name)
        {
            throw new NotImplementedException();
        }

        public bool UpdateContent(string text)
        {
            throw new NotImplementedException();
        }
    }
}
