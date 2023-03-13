using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Items
{
    internal class Book : IBook, INamedItem, IItemContainer<IPage>
    {
        event TypedEventHandler<INamedItem, NamedItemNameChangedEventArgs> INamedItem.ItemNameChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event TypedEventHandler<IItem, ItemDeletedResult> IItem.ItemDeleted
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event TypedEventHandler<IItemContainer<IPage>, ItemContainerCurrentContentEventArgs<IPage>> IItemContainer<IPage>.ItemContainerCurrentContent
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event TypedEventHandler<IItemContainer<IPage>, ItemContainerContentChangedEventArgs<IPage>> IItemContainer<IPage>.ItemContainerContentChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        void IBook.AddPage()
        {
            throw new NotImplementedException();
        }

        void INamedItem.ChangeItemName(string newName)
        {
            throw new NotImplementedException();
        }

        void IItem.DeleteItem()
        {
            throw new NotImplementedException();
        }

        void IItemContainer<IPage>.MakeParentOf(IPage item)
        {
            throw new NotImplementedException();
        }

        void IBook.RemovePage()
        {
            throw new NotImplementedException();
        }

        void IItemContainer<IPage>.RequestItemContainerCurrentContent()
        {
            throw new NotImplementedException();
        }

        void INamedItem.RequestItemNameUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
