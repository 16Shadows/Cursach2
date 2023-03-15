using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Implementation.Organizers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DMOrganizerModel.Implementation.Utility;
using CSToolbox.Weak;

namespace DMOrganizerModel.Implementation.Items
{
    internal class BookPage : ContainerItem<IObjectContainer>, IPage
    {
        public WeakEvent<IPage, PageActionEventArgs> PageActionCompleted { get; } = new();
        private void InvokePageActionCompleted(int parentID, PageActionEventArgs.ActionType action, PageActionEventArgs.ResultType result = PageActionEventArgs.ResultType.Success)
        {
            PageActionCompleted.Invoke(this, new PageActionEventArgs(parentID, action, result));
        }
        // IPage
        public BookPage(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer) { }

        public void AddContainer(int containerType = 1)
        {
            CheckDeleted();
            Task.Run(() =>
            {
                containerType = 1; //other types not implemented yet
                int newContID = -1;
                IObjectContainer item = null;
                lock (Lock)
                {
                    //create container
                    newContID = Query.CreateObjectContainer(Organizer.Connection, containerType);
                    item = Organizer.GetObjectContainer(newContID, this);
                }
                //set this page as parent (M:M bond)
                if (newContID != -1)
                {
                    Query.SetObjectContainerParent(Organizer.Connection, ItemID, newContID);
                    InvokeItemContainerContentChanged(item, ItemContainerContentChangedEventArgs<IObjectContainer>.ChangeType.ItemAdded, ItemContainerContentChangedEventArgs<IObjectContainer>.ResultType.Success);
                }
            });
        }

        public void MovePagesToInsertPage(int BookID, int position)
        {
            CheckDeleted();
            Task.Run(() =>
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
                        ChangePagePosition(BookID, changePositions[i], changePositions[i] + 1);
                    }
                }
            });
        }

        public void MovePagesAfterPageDeletion(int BookID, int position)
        {
            CheckDeleted();
            Task.Run(() =>
            {
                // lock problems , need to make lock somehow                                                                   !!!
                
                //get all page's positions that we need to change (> position)
                List<int> changePositions = Query.GetPagesPositionsToChange(Organizer.Connection, ItemID, position);
                changePositions.Sort();
                //changing positions from beginig to avoid unique pos exception
                for (int i = changePositions.Count; i < changePositions.Count; i++)
                {
                    ChangePagePosition(BookID, changePositions[i], changePositions[i] - 1);
                }
            });
        }
        public void ChangePagePosition(int bookID, int oldPosition, int newPosition)
        {
            CheckDeleted();
            Task.Run(() =>
            {
                //has page check
                if (Query.BookHasPage(Organizer.Connection, bookID, oldPosition))
                {
                    bool res = false;
                    lock (Lock)
                    {
                       res = Query.SetPagePosition(Organizer.Connection, ItemID, oldPosition, newPosition);
                    }
                    if (res )InvokePageActionCompleted(bookID, PageActionEventArgs.ActionType.ChangedPosition, PageActionEventArgs.ResultType.Success);
                    else InvokePageActionCompleted(bookID, PageActionEventArgs.ActionType.ChangedPosition, PageActionEventArgs.ResultType.Failure);
                }
                else
                {
                    InvokePageActionCompleted(bookID, PageActionEventArgs.ActionType.ChangedPosition, PageActionEventArgs.ResultType.Failure);
                    throw new InvalidOperationException("Attempt on moving non existing pages.");
                }
            });

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
            if (parent is not IBook) throw new ArgumentTypeException(nameof(parent), "Unsupported page parent type.");
            else Query.SetPageParent(Organizer.Connection, ItemID, (parent as Book).ItemID);
        }

        protected override bool DeleteItemInternal()
        {
            int parentID = (this.Parent as Book).ItemID;
            int pagePos = RequestPagePosition();
            bool res = Query.DeletePage(Organizer.Connection, ItemID);
            // with page deletion we need to move all other pages after this 1 position back
            if (res) MovePagesAfterPageDeletion(parentID, pagePos);

            return res;
        }
    }
}
