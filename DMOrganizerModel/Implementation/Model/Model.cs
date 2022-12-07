using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Document;
using DMOrganizerModel.Interface.Model;
using DMOrganizerModel.Interface.Reference;
using System;

namespace DMOrganizerModel.Implementation.Model
{
    internal class Model : IModel
    {
        protected Model()
        {

        }

        static Model()
        {
            Instance = new Model();
        }

        public static IModel Instance { get; }

        public event OperationResultEventHandler<IModel, ReferenceDecodedEventArgs>? ReferenceDecoded;
        public event OperationResultEventHandler<IModel, NavigationTreeReceivedEventArgs>? NavigationTreeReceived;
        public event OperationResultEventHandler<IModel>? DataDeleted;

        public IReference CreateReference(ISection section)
        {
            throw new NotImplementedException();
        }

        public bool DecodeReference(string reference)
        {
            throw new NotImplementedException();
        }

        public bool DeleteData()
        {
            throw new NotImplementedException();
        }

        public bool GetNavigationTree()
        {
            throw new NotImplementedException();
        }
    }
}
