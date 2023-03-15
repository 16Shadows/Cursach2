using DMOrganizerModel.Implementation.Organizers;
using DMOrganizerViewModel;
using MVVMToolbox;
using System;

namespace DMOrganizerApp.ViewModels
{
    internal class MainViewModel : ViewModelHost
    {
        private string m_ActiveOrganizerName = "";
        public string ActiveOrganizerName
        {
            get => m_ActiveOrganizerName;
            set
            {
                if (m_ActiveOrganizerName == value)
                    return;

                m_ActiveOrganizerName = value;
                InvokePropertyChanged(nameof(ActiveOrganizerName));
            }
        }

        public MainViewModel(IContext context, IServiceProvider serviceProvider) : base(context, serviceProvider, null)
        {
            m_ActiveOrganizerName = "test.dmo"; //dummy
            ActiveViewModel = new OrganizerViewModel(context, serviceProvider, OrganizersStorageModel.LoadOrganizer(ActiveOrganizerName));
        }
    }
}
