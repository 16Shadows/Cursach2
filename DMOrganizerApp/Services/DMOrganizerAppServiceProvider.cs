using CSToolbox;
using DMOrganizerViewModel;
using MVVMToolbox.Services;
using System;

namespace DMOrganizerApp.Services
{
    internal sealed class DMOrganizerAppServiceProvider : IServiceProvider
    {
        private DMOrganizerAppInputBoxService InputBoxService { get; }
        private DMOrganizerAppNotificationService NotificationService { get; }

        public DMOrganizerAppServiceProvider()
        {
            InputBoxService = new DMOrganizerAppInputBoxService();
            NotificationService = new DMOrganizerAppNotificationService();
        }

        public object? GetService(Type serviceType)
        {
            if (serviceType.Is(typeof(INotificationService<OrganizerNotificationScenarios>)))
                return NotificationService;
            else if (serviceType.Is(typeof(IInputBoxService<OrganizerInputBoxScenarios>)))
                return InputBoxService;
            return null;
        }
    }
}
