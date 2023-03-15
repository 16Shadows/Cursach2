using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using System;

namespace DMOrganizerViewModel.Test
{
    public class CategoryViewModel : NamedContainerViewModel<IOrganizerItem>
    {
        public CategoryViewModel(IContext context, IServiceProvider serviceProvider, ICategory category) : base(context, serviceProvider, category, category) {}
    }
}
