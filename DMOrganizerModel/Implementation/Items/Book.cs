using CSToolbox;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Implementation.Organizers;
using DMOrganizerModel.Interface.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Runtime.CompilerServices;

namespace DMOrganizerModel.Implementation.Items
{
    internal class Book : NamedContainerItem<IPage>, IBook
    {
        //IBook
        public Book(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer) {}
        
        // maybe need to remove at all                      !!!!!
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
                int availablePosition = Query.MaxPagePosition(Organizer.Connection, ItemID) + 1;
                IPage item = null;
                lock (Lock)
                {
                    int newPageID = Query.CreatePadeInBook(Organizer.Connection, ItemID, availablePosition);
                    item = Organizer.GetPage(newPageID, this); //caching object
                }
                // telling everyone that page is added
                InvokeItemContainerContentChanged(item, ItemContainerContentChangedEventArgs<IPage>.ChangeType.ItemAdded, ItemContainerContentChangedEventArgs<IPage>.ResultType.Success);

            });
        }

        public void AddPage(int position)
        {
            //check how many pages, if it is last or bigger - add at last,
            //if in middle - move all other pages(from end to avoid non-unique positions) +1 and add this page
            CheckDeleted();
            Task.Run(() =>
            {
                throw new NotImplementedException();
                lock (Lock) 
                { 

                }
                //InvokeBookItemCreated()
                int newPageID = Query.CreatePadeInBook(Organizer.Connection, ItemID, position);
                IPage item = null;
                item = Organizer.GetPage(newPageID, this); //caching object

                // telling everyone that page is added
                InvokeItemContainerContentChanged(item, ItemContainerContentChangedEventArgs<IPage>.ChangeType.ItemAdded, ItemContainerContentChangedEventArgs<IPage>.ResultType.Success);

            });
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

        protected override bool HasItem(IPage item) //checks if has page with some ID in it
        {
           return item is BookPage page && Query.BookHasPage(Organizer.Connection, ItemID, page.ItemID);
        }

        // Need to overwrite
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
