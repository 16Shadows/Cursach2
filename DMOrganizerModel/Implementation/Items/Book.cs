using CSToolbox;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Implementation.Organizers;
using DMOrganizerModel.Interface.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                //InvokeBookItemCreated()
                int newPageID = Query.CreatePadeInBook(Organizer.Connection, ItemID, availablePosition);
                IItem item = null;
                item = Organizer.GetPage(newPageID, this);
                //InvokeItemContainerContentChanged()

            });
        }

        public void AddPage(int position)
        {
            //check how many pages, if it is last or bigger - add at last,
            //if in middle - move all other pages(from end to avoid non-unique positions) +1 and add this page
            throw new NotImplementedException();
        }

        public void ChangePagePosition(int oldPosition, int newPosition)
        {
            throw new NotImplementedException();
        }

        public override string GetName()
        {
            throw new NotImplementedException();
        }

        public void RemovePage(int position)
        {
            throw new NotImplementedException();
        }

        //INamedContainerItem
        public override void SetName(string name)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IPage> GetContent()
        {
            throw new NotImplementedException();
        }

        protected override bool HasItem(IPage item) //checks if has page with some ID in it
        {
            throw new NotImplementedException();
        }

        // Need to overwrite
        protected override void SetParentInternal(IItemContainerBase parent)
        {
            throw new NotImplementedException();
        }

        protected override bool DeleteItemInternal()
        {
            throw new NotImplementedException();
        }

        //
    }
}
