using CSToolbox.Extensions;
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
            if (serviceType.Is(typeof(INotificationService<OrganizerNotificationScenarios>)) ||
                serviceType.Is(typeof(INotificationService<CategoryNotificationScenarios>)) ||
                serviceType.Is(typeof(INotificationService<NamedItemNotificationScenarios>)))
                return NotificationService;
            else if (serviceType.Is(typeof(IInputBoxService<OrganizerInputBoxScenarios>)) ||
                     serviceType.Is(typeof(IInputBoxService<CategoryInputBoxScenarios>)) ||
                     serviceType.Is(typeof(IInputBoxService<NamedItemInputBoxScenarios>)))
                return InputBoxService;
            return null;
        }
    }
}
