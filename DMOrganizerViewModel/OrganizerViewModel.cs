using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Interface.Organizer;
using MVVMToolbox;
using System;
using System.Collections.ObjectModel;

namespace DMOrganizerViewModel
{
    public sealed class OrganizerViewModel : ContainerViewModel<IOrganizerItem>
    {
        private IOrganizer Organizer { get; }
        public ObservableCollection<ViewModelBase> OpenedItems { get; } = new();

        public OrganizerViewModel(IContext context, IServiceProvider serviceProvider, IOrganizer organizer) : base(context, serviceProvider, organizer)
        {
            Organizer = organizer ?? throw new ArgumentNullException(nameof(organizer));
        }

        protected override ViewModelBase CreateViewModel(IOrganizerItem item)
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
