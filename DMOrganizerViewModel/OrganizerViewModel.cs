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
            Organizer = organizer;
        }
    }
}
