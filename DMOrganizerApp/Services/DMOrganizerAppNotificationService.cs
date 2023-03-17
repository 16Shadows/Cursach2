using DMOrganizerViewModel;
using MVVMToolbox;
using MVVMToolbox.Services;
using System;
using System.Windows;

namespace DMOrganizerApp.Services
{
    internal sealed class DMOrganizerAppNotificationService : INotificationService<OrganizerNotificationScenarios>
    {
        public void Show(NotificationConfiguration<OrganizerNotificationScenarios> configuration)
        {
            MessageBox.Show("Ha-ha!");
        }
    }
}
