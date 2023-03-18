using DMOrganizerApp.Resources;
using DMOrganizerViewModel;
using MVVMToolbox.Services;
using System.Windows;

namespace DMOrganizerApp.Services
{
    internal sealed class DMOrganizerAppNotificationService : INotificationService<OrganizerNotificationScenarios>, 
                                                              INotificationService<CategoryNotificationScenarios>,
                                                              INotificationService<NamedItemNotificationScenarios>,
                                                              INotificationService<DocumentNotificationScenarios>
    {
        public void Show(NotificationConfiguration<OrganizerNotificationScenarios> configuration)
        {
            if (configuration is not OrganizerNotificationConfiguration config)
                return;
            else if (config.Scenario == OrganizerNotificationScenarios.CreateCategorySuccess)
                MessageBox.Show(Application.Current.MainWindow, string.Format(LocalizedStrings.CategoryCreated, config.Name));
            else if (config.Scenario == OrganizerNotificationScenarios.CreateDocumentSuccess)
                MessageBox.Show(Application.Current.MainWindow, string.Format(LocalizedStrings.DocumentCreated, config.Name));
            else if (config.Scenario == OrganizerNotificationScenarios.CreateBookSuccess)
                MessageBox.Show(Application.Current.MainWindow, string.Format(LocalizedStrings.BookCreated, config.Name));

            else if (config.Scenario == OrganizerNotificationScenarios.DuplicateItemName)
                MessageBox.Show(Application.Current.MainWindow, string.Format(LocalizedStrings.DuplicateItemName, config.Name));
        }

        public void Show(NotificationConfiguration<CategoryNotificationScenarios> configuration)
        {
            if (configuration is not CategoryNotificationConfiguration config)
                return;
            else if (config.Scenario == CategoryNotificationScenarios.CreateCategorySuccess)
                MessageBox.Show(Application.Current.MainWindow, string.Format(LocalizedStrings.CategoryCreated, config.Name));
            else if (config.Scenario == CategoryNotificationScenarios.CreateDocumentSuccess)
                MessageBox.Show(Application.Current.MainWindow, string.Format(LocalizedStrings.DocumentCreated, config.Name));
            else if (config.Scenario == CategoryNotificationScenarios.CreateBookSuccess)
                MessageBox.Show(Application.Current.MainWindow, string.Format(LocalizedStrings.BookCreated, config.Name));

            else if (config.Scenario == CategoryNotificationScenarios.DuplicateItemName)
                MessageBox.Show(Application.Current.MainWindow, string.Format(LocalizedStrings.DuplicateItemName, config.Name));
        }

        public void Show(NotificationConfiguration<NamedItemNotificationScenarios> configuration)
        {
            if (configuration is not NamedItemNotificationConfiguration config)
                return;
            else if (config.Scenario == NamedItemNotificationScenarios.ItemRenamed)
                MessageBox.Show(Application.Current.MainWindow, string.Format(LocalizedStrings.RenamedSuccessfully, config.Name));
            else if (config.Scenario == NamedItemNotificationScenarios.DuplicateItemName)
                MessageBox.Show(Application.Current.MainWindow, string.Format(LocalizedStrings.DuplicateItemName, config.Name));
            else if (config.Scenario == NamedItemNotificationScenarios.ItemDeleted)
                MessageBox.Show(Application.Current.MainWindow, string.Format(LocalizedStrings.DeletedItem, config.Name));
        }

        public void Show(NotificationConfiguration<DocumentNotificationScenarios> configuration)
        {
            if (configuration is not DocumentNotificationConfiguration config)
                return;
            else if (config.Scenario == DocumentNotificationScenarios.DuplicateTag)
                MessageBox.Show(Application.Current.MainWindow, string.Format(LocalizedStrings.DuplicateTag, config.Tag));
        }
    }
}
