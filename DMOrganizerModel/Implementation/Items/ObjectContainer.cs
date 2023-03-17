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
        private void InvokeObjectContainerViewInfo(int width, int height, int coordX, int coordY, int type)
        {
            ObjectContainerViewInfo.Invoke(this, new ObjectContainerViewInfoEventArgs(width, height, coordX, coordY, type));
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

        /// <summary>
        /// Get container view info (width, height, X, Y)
        /// </summary>
        /// <returns></returns>
        public List<int> RequestContainerViewInfo()
        {
            List<int> info = Query.GetContainerViewInfo(Organizer.Connection, ItemID);
            InvokeObjectContainerViewInfo(info[0], info[1], info[2], info[3]);
            return info;
        }

        public void UpdateCoordinates(int newX, int newY)
        {
            int oldX = Query.GetContainerViewInfo(Organizer.Connection, ItemID)[2];
            int oldY = Query.GetContainerViewInfo(Organizer.Connection, ItemID)[3];
            CheckDeleted();
            Task.Run(() =>
            {
                bool res = false;
                lock(Lock)
                {
                    res = Query.SetContainerCoordinates(Organizer.Connection, ItemID, newX, newY);
                }
                if (res) InvokeObjectContainerUpdatedPosition(newX, newY, ObjectContainerUpdatePositionEventArgs.ResultType.Success);
                else InvokeObjectContainerUpdatedPosition(oldX, oldY, ObjectContainerUpdatePositionEventArgs.ResultType.IncorrectCoordinates);
            });
        }

        public void UpdateSize(int newWidth, int newHeight)
        {
            int oldWidth = Query.GetContainerViewInfo(Organizer.Connection, ItemID)[0];
            int oldHeight = Query.GetContainerViewInfo(Organizer.Connection, ItemID)[1];
            CheckDeleted();
            Task.Run(() =>
            {
                bool res = false;
                lock (Lock)
                {
                    res = Query.SetContainerCoordinates(Organizer.Connection, ItemID, newWidth, newHeight);
                }
                if (res) InvokeObjectContainerUpdatedSize(newWidth, newHeight, ObjectContainerUpdateSizeEventArgs.ResultType.Success);
                else InvokeObjectContainerUpdatedSize(newWidth, newHeight, ObjectContainerUpdateSizeEventArgs.ResultType.IncorrectSize);
            });
        }

        //public void UpdateContent(int oldObjectID, int newObjectID)
        //{
        //    //oldObjectID is for multiple-object containers implementation
        //    //old object will be deleted maybe
        //    CheckDeleted();
        //    Task.Run(() =>
        //    {
        //        bool res = Query.ContainerHasObject(Organizer.Connection, ItemID, oldObjectID);
        //        if (res)
        //        {
        //            lock (Lock)
        //            {
        //                ContainerObject obj = Organizer.GetObject
        //            }
        //        }
        //    });

        //}

        protected override IEnumerable<IObject> GetContent()
        {
            List<IObject> result = new List<IObject>();
            foreach (int objID in Query.GetContainerContent(Organizer.Connection, ItemID))
                result.Add(Organizer.GetObject(objID, this));

            return result;
        }

        protected override bool HasItem(IObject item)
        {
            return item is ContainerObject obj && Query.ContainerHasObject(Organizer.Connection, ItemID, obj.ItemID);
        }

        protected override void SetParentInternal(IItemContainerBase parent)
        {
            if (parent is not BookPage) throw new ArgumentTypeException(nameof(parent), "Unsupported container parent type.");
            else Query.SetContainerParent(Organizer.Connection, ItemID, (parent as BookPage).ItemID);
        }
        protected override bool DeleteItemInternal()
        {
            return Query.DeleteObjectContainer(Organizer.Connection, ItemID);
        }

    }
}
