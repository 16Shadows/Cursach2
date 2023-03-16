using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using MVVMToolbox.ViewModel;
using System;

namespace DMOrganizerViewModel
{
    public class CategoryViewModel : NamedContainerViewModel<IOrganizerItem>
    {
        public CategoryViewModel(IContext context, IServiceProvider serviceProvider, ICategory category) : base(context, serviceProvider, category, category) {}

        protected override DMOrganizerViewModelBase CreateViewModel(IOrganizerItem item)
        {
            if (item is ICategory category)
                return new CategoryViewModel(Context, ServiceProvider, category);
            else if (item is IDocument document)
                return new DocumentViewModel(Context, ServiceProvider, document);
            else
                throw new ArgumentException("Unsupported item type", nameof(item));
        }
    }
}
