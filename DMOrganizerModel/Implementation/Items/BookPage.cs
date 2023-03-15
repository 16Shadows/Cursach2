using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Implementation.Organizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMOrganizerModel.Implementation.Utility;
using System.Windows.Controls;

namespace DMOrganizerModel.Implementation.Items
{
    internal class BookPage : ContainerItem<IObjectContainer>, IPage
    {
        // IPage
        public BookPage(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer) { }

        public void ChangePagePosition(int bookID, int oldPosition, int newPosition)
        {
            throw new NotImplementedException();
            CheckDeleted();
            Task.Run(() =>
            {
                //haspage check проверить позицию, что есть такая страница в книге, айди не важен
                //int currPageID = Query.GetPageIDByPosition(Organizer.Connection, ItemID, oldPosition);
                Query.SetPagePosition(Organizer.Connection, ItemID, oldPosition, newPosition);
                //InvokeItemContainerContentChanged();
            });

        }

        public void AddContainer(int containerType = 1)
        {
            containerType = 1; //other types not implemented yet
            IObjectContainer item = null;
            lock (Lock)
            {
                //create container
                int newContID = Query.CreateObjectContainer(Organizer.Connection, containerType);
                item = Organizer.GetObjectContainer(newContID, this);
                //set this page as parent (M:M bond)
                if (newContID != -1)
                {
                    Query.SetObjectContainerParent(Organizer.Connection, ItemID, newContID);
                    InvokeItemContainerContentChanged(item, ItemContainerContentChangedEventArgs<IObjectContainer>.ChangeType.ItemAdded, ItemContainerContentChangedEventArgs<IObjectContainer>.ResultType.Success);
                }
            }
        }

        public void MovePagesToInsertPage(int BookID, int position)
        {
            lock (Lock)
            {
                //get all page's positions that we need to change (>= position)
                List<int> changePositions = Query.GetPagesPositionsToChange(Organizer.Connection, ItemID, position);
                changePositions.Sort();
                changePositions.Reverse();
                //changing positions from end to avoid unique pos exception
                for (int i = changePositions.Count; i < changePositions.Count; i++)
                {
                    ChangePagePosition(ItemID, changePositions[i], changePositions[i] + 1);
                }
            }
        }

        //request page position no matter in what book it is, just by id
        public int RequestPagePosition()
        {
            return Query.GetPagePosition(Organizer.Connection, ItemID);
        }

        protected override bool HasItem(IObjectContainer item)
        {
            return item is ObjectContainer cont && Query.PageHasContainer(Organizer.Connection, ItemID, cont.ItemID);
        }

        protected override IEnumerable<IObjectContainer> GetContent()
        {
            List<IObjectContainer> result = new List<IObjectContainer>();
            foreach (int containerID in Query.GetPageContent(Organizer.Connection, ItemID))
                result.Add(Organizer.GetObjectContainer(containerID, this));
            return result;
        }

        protected override void SetParentInternal(IItemContainerBase parent)
        {
            if (parent is not Book) throw new ArgumentTypeException(nameof(parent), "Unsupported page parent type.");
            else Query.SetPageParent(Organizer.Connection, ItemID, (parent as Book).ItemID);
        }

        protected override bool DeleteItemInternal()
        {
            return Query.DeletePage(Organizer.Connection, ItemID);
        }
    }
}
