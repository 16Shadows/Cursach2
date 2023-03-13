using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using System;
using System.Collections.ObjectModel;

namespace DMOrganizerViewModel
{
    public class CategoryViewModel : ViewModelBase
    {
        private bool m_ItemsRequestStarted;
        private ObservableCollection<IOrganizerItem>? m_Items;
        public ObservableCollection<IOrganizerItem>? Items
        {
            get
            {
                if (!m_ItemsRequestStarted && m_Items == null)
                {
                    m_ItemsRequestStarted = true;
                    Category.RequestItemContainerCurrentContent();
                }
                return m_Items;
            }
            private set
            {
                if (m_Items == value)
                    return;

                m_Items = value;
                InvokePropertyChanged(nameof(Items));
            }
        }

        private string? m_Name;
        public string? Name
        {
            get => m_Name;
            private set
            {
                if (m_Name == value)
                    return;

                m_Name = value;
                InvokePropertyChanged(nameof(Name));
            }
        }

        protected ICategory Category { get; }

        public CategoryViewModel(IContext context, IServiceProvider serviceProvider, ICategory category) : base(context, serviceProvider)
        {
            Category = category ?? throw new ArgumentNullException(nameof(category));
            m_ItemsRequestStarted = false;

            category.ItemContainerCurrentContent.Subscribe(Category_ItemContainerCurrentContent);
            category.ItemNameChanged.Subscribe(Category_ItemNameChanged);
            category.ItemContainerContentChanged.Subscribe(Category_ItemContainerContentChanged);

            category.RequestItemNameUpdate();
        }

        private void Category_ItemContainerContentChanged(IItemContainer<IOrganizerItem> sender, ItemContainerContentChangedEventArgs<IOrganizerItem> e)
        {
            if (!e.HasChanged || m_Items == null)
                return;
            else if (e.Type == ItemContainerContentChangedEventArgs<IOrganizerItem>.ChangeType.ItemAdded)
                Items.Add(e.Item);
            else
                Items.Remove(e.Item);
        }

        private void Category_ItemNameChanged(INamedItem sender, NamedItemNameChangedEventArgs e)
        {
            if (!e.HasChanged && Name != null)
                return;
            Name = e.Name;
        }

        private void Category_ItemContainerCurrentContent(IItemContainer<IOrganizerItem> sender, ItemContainerCurrentContentEventArgs<IOrganizerItem> e)
        {
            if (!m_ItemsRequestStarted)
                return;
            m_ItemsRequestStarted = false;
            Items = new ObservableCollection<IOrganizerItem>(e.Content);
        }
    }
}
