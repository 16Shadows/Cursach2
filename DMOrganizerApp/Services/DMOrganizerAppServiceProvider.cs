﻿using CSToolbox.Extensions;
using DMOrganizerViewModel;
using MVVMToolbox.Services;
using System;

namespace DMOrganizerApp.Services
{
    internal sealed class DMOrganizerAppServiceProvider : IServiceProvider
    {
        private DMOrganizerAppInputBoxService InputBoxService { get; }
        private DMOrganizerAppNotificationService NotificationService { get; }
        private ReferenceSelector ReferenceSelectorService { get; }

        public DMOrganizerAppServiceProvider()
        {
            InputBoxService = new DMOrganizerAppInputBoxService();
            NotificationService = new DMOrganizerAppNotificationService();
            ReferenceSelectorService = new ReferenceSelector();
        }

        public object? GetService(Type serviceType)
        {
            if (serviceType.Is(typeof(INotificationService<OrganizerNotificationScenarios>)) ||
                serviceType.Is(typeof(INotificationService<CategoryNotificationScenarios>)) ||
                serviceType.Is(typeof(INotificationService<NamedItemNotificationScenarios>)) ||
                serviceType.Is(typeof(INotificationService<DocumentNotificationScenarios>)) ||
                serviceType.Is(typeof(INotificationService<SectionNotificationScenarios>)))
                return NotificationService;
            else if (serviceType.Is(typeof(IInputBoxService<OrganizerInputBoxScenarios>)) ||
                     serviceType.Is(typeof(IInputBoxService<CategoryInputBoxScenarios>)) ||
                     serviceType.Is(typeof(IInputBoxService<NamedItemInputBoxScenarios>)) ||
                     serviceType.Is(typeof(IInputBoxService<DocumentInputBoxScenarios>)) ||
                     serviceType.Is(typeof(IInputBoxService<SectionInputBoxScenarios>)))
                return InputBoxService;
            else if (serviceType.Is(typeof(IReferenceSelector)))
                return ReferenceSelectorService;
            return null;
        }
    }
}
