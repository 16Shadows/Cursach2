using CSToolbox;
using CSToolbox.Weak;
using DMOrganizerModel.Implementation.Organizers;
using DMOrganizerModel.Interface.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private void InvokeBookItemCreated(int width, int height, ObjectContainerUpdateSizeEventArgs.ResultType result)
        {
            ObjectContainerUpdatedSize.Invoke(this, new ObjectContainerUpdateSizeEventArgs(width, height, result));
        }
        public WeakEvent<IObjectContainer, ObjectContainerViewInfoEventArgs> ObjectContainerViewInfo { get; } = new();
        private void InvokeBookItemCreated(int width, int height, int coordX, int coordY)
        {
            ObjectContainerViewInfo.Invoke(this, new ObjectContainerViewInfoEventArgs(width, height, coordX, coordY));
        }

        public void AddObject()
        {
            throw new NotImplementedException();
        }

        public void RequestContainerViewInfo()
        {
            throw new NotImplementedException();
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
