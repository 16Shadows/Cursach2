using DMOrganizerModel.Interface.Document;
using DMOrganizerModel.Interface.Model;
using DMOrganizerModel.Interface.NavigationTree;
using DMOrganizerModel.Interface.Reference;
using System;

namespace DMOrganizerModel.Implementation.Model
{
    internal class Model : IModel
    {
        public IReference CreateReference(ISection section)
        {
            throw new NotImplementedException();
        }

        public IReference DecodeReference(string reference)
        {
            throw new NotImplementedException();
        }

        public INavigationTreeCategory GetNavigationTree()
        {
            throw new NotImplementedException();
        }
    }
}
