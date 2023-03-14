using CSToolbox;
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

        public WeakEvent<IObjectContainer, ObjectContainerUpdatePositionEventArgs> ObjectContainerUpdatedPosition => throw new NotImplementedException();

        public WeakEvent<IObjectContainer, ObjectContainerUpdateSizeEventArgs> ObjectContainerUpdatedSize => throw new NotImplementedException();

        public WeakEvent<IObjectContainer, ObjectContainerViewInfoEventArgs> ObjectContainerViewInfo => throw new NotImplementedException();

        public void CreateObject()
        {
            throw new NotImplementedException();
        }

        public void RequestContainerViewInfo()
        {
            throw new NotImplementedException();
        }

        public void UpdateContent(IObject obj)
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
