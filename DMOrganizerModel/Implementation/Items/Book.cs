using CSToolbox.Weak;
using DMOrganizerModel.Implementation.Organizers;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface.Items;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Items
{
    internal class Book : NamedContainerItem<IPage>, IBook
    {
        //IBook
        public Book(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer) {}

        public WeakEvent<IBook, BookItemCreatedEventArgs> BookItemCreated { get; } = new();

        private void InvokeBookItemCreated(int ID, BookItemCreatedEventArgs.ResultType result)
        {
            BookItemCreated.Invoke(this, new BookItemCreatedEventArgs(ID, result));
        }

        public void AddPage()
        {
            //check what last page position is, add page at last+1 position, if none - will be first page
            CheckDeleted();
            Task.Run(() =>
            {
                IPage item = null;
                int newPageID;
                int availablePosition = Query.MaxPagePosition(Organizer.Connection, ItemID) + 1;
                lock (Lock)
                {
                    newPageID = Query.CreatePadeInBook(Organizer.Connection, ItemID, availablePosition);
                }
                if (newPageID != -1)
                {
                    item = Organizer.GetPage(newPageID, this); //caching object
                    // telling everyone that page is added
                    InvokeItemContainerContentChanged(item, ItemContainerContentChangedEventArgs<IPage>.ChangeType.ItemAdded, ItemContainerContentChangedEventArgs<IPage>.ResultType.Success);
                    InvokeBookItemCreated(newPageID, BookItemCreatedEventArgs.ResultType.Success);
                }
                else
                {
                    InvokeBookItemCreated(newPageID, BookItemCreatedEventArgs.ResultType.Failure);
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
                        int id = Query.GetPageID(Organizer.Connection, ItemID, changePositions[i]);
                        BookPage p = Organizer.GetPage(id, this);
                        p.ChangePagePosition(BookID, changePositions[i], changePositions[i] + 1);
                    }
                }
            });
        }
        public int GetLastAvailablePagePosition(int BookID)
        {
            CheckDeleted();
            return Query.MaxPagePosition(Organizer.Connection, BookID)+1;
        }
        public void AddPage(int position)
        {
            //check how many pages, if it is last or bigger - add at last,
            //if in middle - move all other pages(from end to avoid non-unique positions) +1 and add this page
            CheckDeleted();
            int newPageID = -1;
            IPage item = null;

            Task.Run(() =>
            {
                MovePagesToInsertPage(ItemID, position); //need check if pages were moved
                newPageID = Query.CreatePadeInBook(Organizer.Connection, ItemID, position);
                item = Organizer.GetPage(newPageID, this); //caching object
            });
            if (newPageID != -1)
            { 
                // telling everyone that page is added
                InvokeItemContainerContentChanged(item, ItemContainerContentChangedEventArgs<IPage>.ChangeType.ItemAdded, ItemContainerContentChangedEventArgs<IPage>.ResultType.Success);
            }

        }

        public override string GetName()
        {
            return Query.GetBookName(Organizer.Connection, ItemID);
        }

        //INamedContainerItem        
        public override void SetName(string name)
        {
            _ = Query.SetBookName(Organizer.Connection, ItemID, name); // _ is to discard useless variable
        }

        protected override IEnumerable<IPage> GetContent()
        {
            //get pages from database and checks in page's cache 
            List<IPage> result = new List<IPage>();
            foreach (int pageID in Query.GetBookContent(Organizer.Connection, ItemID))
                result.Add(Organizer.GetPage(pageID, this));

            return result;
        }

        protected override void SetParentInternal(IItemContainerBase parent)
        {
            if (parent is null) Query.SetBookParent(Organizer.Connection, ItemID);
            else if (parent is not Category) throw new ArgumentTypeException(nameof(parent), "Unsupported book parent type.");
            else Query.SetBookParent(Organizer.Connection, ItemID, (parent as Category).ItemID);
        }

        protected override bool DeleteItemInternal()
        {
            return Query.DeleteBook(Organizer.Connection, ItemID);
        }
    }
}
