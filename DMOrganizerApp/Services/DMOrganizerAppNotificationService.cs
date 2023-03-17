using DMOrganizerViewModel;
using MVVMToolbox;
using MVVMToolbox.Services;
using System;

namespace DMOrganizerApp.Services
{
    internal sealed class DMOrganizerAppNotificationService : INotificationService<OrganizerNotificationScenarios>
    {
        public void Show(NotificationConfiguration<OrganizerNotificationScenarios> configuration)
        {
            throw new NotImplementedException();
        }
    }
}
