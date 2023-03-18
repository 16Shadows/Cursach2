using DMOrganizerApp.Resources;
using DMOrganizerApp.UserControls;
using DMOrganizerViewModel;
using MVVMToolbox.Services;
using System.Globalization;
using System.Windows;

namespace DMOrganizerApp.Services
{
    internal class DMOrganizerAppInputBoxService : IInputBoxService<OrganizerInputBoxScenarios>, IInputBoxService<CategoryInputBoxScenarios>, IInputBoxService<NamedItemInputBoxScenarios>
    {
        public InputBoxResult Show<ReturnType>(InputBoxConfiguration<OrganizerInputBoxScenarios, ReturnType> configuration)
        {
            ExclusiveInputBox box = new ()
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Validator = (value, culture) => configuration.Validator(value, culture)
            };
            if (configuration.Scenario == OrganizerInputBoxScenarios.CategoryName)
                box.InputPrompt = LocalizedStrings.InputCategoryName;
            else if (configuration.Scenario == OrganizerInputBoxScenarios.DocumentName)
                box.InputPrompt = LocalizedStrings.InputDocumentName;
            else if (configuration.Scenario == OrganizerInputBoxScenarios.BookName)
                box.InputPrompt = LocalizedStrings.InputBookName;

            if (box.ShowDialog() != true)
                return InputBoxResult.Canceled;
            configuration.UserInput = configuration.Converter(box.Text, CultureInfo.CurrentCulture);
            return InputBoxResult.Success;
        }

        public InputBoxResult Show<ReturnType>(InputBoxConfiguration<CategoryInputBoxScenarios, ReturnType> configuration)
        {
            ExclusiveInputBox box = new ()
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Validator = (value, culture) => configuration.Validator(value, culture)
            };
            if (configuration.Scenario == CategoryInputBoxScenarios.CategoryName)
                box.InputPrompt = LocalizedStrings.InputCategoryName;
            else if (configuration.Scenario == CategoryInputBoxScenarios.DocumentName)
                box.InputPrompt = LocalizedStrings.InputDocumentName;
            else if (configuration.Scenario == CategoryInputBoxScenarios.BookName)
                box.InputPrompt = LocalizedStrings.InputBookName;

            if (box.ShowDialog() != true)
                return InputBoxResult.Canceled;
            configuration.UserInput = configuration.Converter(box.Text, CultureInfo.CurrentCulture);
            return InputBoxResult.Success;
        }

        public InputBoxResult Show<ReturnType>(InputBoxConfiguration<NamedItemInputBoxScenarios, ReturnType> configuration)
        {
            ExclusiveInputBox box = new ()
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Validator = (value, culture) => configuration.Validator(value, culture)
            };
            if (configuration.Scenario == NamedItemInputBoxScenarios.Rename)
                box.InputPrompt = LocalizedStrings.InputNewName;

            if (box.ShowDialog() != true)
                return InputBoxResult.Canceled;
            configuration.UserInput = configuration.Converter(box.Text, CultureInfo.CurrentCulture);
            return InputBoxResult.Success;
        }
    }
}
