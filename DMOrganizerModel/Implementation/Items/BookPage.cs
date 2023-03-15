using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Implementation.Organizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMOrganizerModel.Implementation.Utility;

namespace DMOrganizerModel.Implementation.Items
{
    internal class BookPage : ContainerItem<IObjectContainer>, IPage
    {
        // IPage
        public BookPage(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer) { }

        public void ChangePagePosition(int bookID, int oldPosition, int newPosition)
        {
            CheckDeleted();
            Task.Run(() =>
            {
                //haspage check проверить позицию, что есть такая страница в книге, айди не важен
                //int currPageID = Query.GetPageIDByPosition(Organizer.Connection, ItemID, oldPosition);
                Query.SetPagePosition(Organizer.Connection, ItemID, oldPosition, newPosition);
                InvokeItemContainerContentChanged()
            });

        }

        public void AddContainer()
        {
            throw new NotImplementedException();
        }

        public void ChangePagePosition(int oldPosition, int newPosition)
        {
            throw new NotImplementedException();
        }

        public void MovePagesToInsertPage(int BookID, int position)
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

        public void RequestPagePosition()
        {
            throw new NotImplementedException();
        }

        public override string GetName()
        {
            throw new NotImplementedException();
        }

        public override void SetName(string name)
        {
            throw new NotImplementedException();
        }

        protected override bool HasItem(IObjectContainer item)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IObjectContainer> GetContent()
        {
            throw new NotImplementedException();
        }

        protected override void SetParentInternal(IItemContainerBase parent)
        {
            throw new NotImplementedException();
        }

        protected override bool DeleteItemInternal()
        {
            throw new NotImplementedException();
        }
    }
}
