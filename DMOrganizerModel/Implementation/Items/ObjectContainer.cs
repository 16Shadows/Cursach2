using CSToolbox;
using CSToolbox.Weak;
using DMOrganizerModel.Implementation.Organizers;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Interface.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace DMOrganizerModel.Implementation.Items
{
    internal class ObjectContainer : ContainerItem<IObject>, IObjectContainer
    {
        public ObjectContainer(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer) {}

        public WeakEvent<IObjectContainer, ObjectContainerUpdatePositionEventArgs> ObjectContainerUpdatedPosition { get; } = new();
        private void InvokeObjectContainerUpdatedPosition(int coordX, int coordY, ObjectContainerUpdatePositionEventArgs.ResultType result)
        {
            ObjectContainerUpdatedPosition.Invoke(this, new ObjectContainerUpdatePositionEventArgs(coordX, coordY, result));
        }

        public WeakEvent<IObjectContainer, ObjectContainerUpdateSizeEventArgs> ObjectContainerUpdatedSize { get; } = new();
        private void InvokeObjectContainerUpdatedSize(int width, int height, ObjectContainerUpdateSizeEventArgs.ResultType result)
        {
            ObjectContainerUpdatedSize.Invoke(this, new ObjectContainerUpdateSizeEventArgs(width, height, result));
        }
        public WeakEvent<IObjectContainer, ObjectContainerViewInfoEventArgs> ObjectContainerViewInfo { get; } = new();
        private void InvokeObjectContainerViewInfo(int width, int height, int coordX, int coordY)
        {
            ObjectContainerViewInfo.Invoke(this, new ObjectContainerViewInfoEventArgs(width, height, coordX, coordY));
        }

        public void AddObject(IReferenceable obj)
        {
            CheckDeleted();
            Task.Run(() =>
            {
                IObject item = null;
                int newObjectID;
                string link = obj.GetReference().Encode();
                lock (Lock)
                {
                    newObjectID = Query.CreateObject(Organizer.Connection, link);
                }
                if (newObjectID != -1)
                {
                    item = Organizer.GetObject(newObjectID, this); //caching object
                    // telling everyone that object is added
                    InvokeItemContainerContentChanged(item, ItemContainerContentChangedEventArgs<IObject>.ChangeType.ItemAdded, ItemContainerContentChangedEventArgs<IObject>.ResultType.Success);
                }
            });
        }

        public void RequestContainerViewInfo()
        {
            List<int> info = Query.GetContainerViewInfo(Organizer.Connection, ItemID);

        }

        public void UpdateContent(int oldObjectID, int newObjectID)
        {
            throw new NotImplementedException();
        }

        public void UpdateCoordinates(int newX, int newY)
        {
            throw new NotImplementedException();
        }

        public void UpdateSize(int newWidth, int newHeight)
        {
            throw new NotImplementedException();
        }

        protected override bool DeleteItemInternal()
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IObject> GetContent()
        {
            throw new NotImplementedException();
        }

        protected override bool HasItem(IObject item)
        {
            throw new NotImplementedException();
        }

        protected override void SetParentInternal(IItemContainerBase parent)
        {
            throw new NotImplementedException();
        }
    }
}
