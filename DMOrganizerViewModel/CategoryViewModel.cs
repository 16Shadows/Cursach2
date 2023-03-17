using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using MVVMToolbox.ViewModel;
using System;
using WPFToolbox.Command;

namespace DMOrganizerViewModel
{
    public class CategoryViewModel : NamedContainerViewModel<IOrganizerItem>
    {
        public DeferredCommand CreateCategory { get; }
        public DeferredCommand CreateDocument { get; }
        public DeferredCommand Rename { get; }
        public DeferredCommand Delete { get; }

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
