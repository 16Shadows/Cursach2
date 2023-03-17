using DMOrganizerApp.Resources;
using DMOrganizerViewModel;
using MVVMToolbox.Services;
using System.Windows;

namespace DMOrganizerApp.Services
{
    internal sealed class DMOrganizerAppNotificationService : INotificationService<OrganizerNotificationScenarios>
    {
        public void Show(NotificationConfiguration<OrganizerNotificationScenarios> configuration)
        {
            if (configuration is not OrganizerNotificationConfiguration config)
                return;
            else if (config.Scenario == OrganizerNotificationScenarios.CreateCategorySuccess)
                MessageBox.Show(Application.Current.MainWindow, string.Format(LocalizedStrings.CategoryCreated, config.Name));
            else if (config.Scenario == OrganizerNotificationScenarios.CreateDocumentSuccess)
                MessageBox.Show(Application.Current.MainWindow, string.Format(LocalizedStrings.DocumentCreated, config.Name));
            else if (config.Scenario == OrganizerNotificationScenarios.DuplicateItemName)
                MessageBox.Show(Application.Current.MainWindow, string.Format(LocalizedStrings.DuplicateItemName, config.Name));
        }
    }
}
