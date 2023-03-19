using DMOrganizerApp.Resources;
using DMOrganizerViewModel;
using MVVMToolbox.Services;
using WPFToolbox.Controls.Dialog;
using System.Globalization;
using System.Windows;

namespace DMOrganizerApp.Services
{
    internal class DMOrganizerAppInputBoxService : IInputBoxService<OrganizerInputBoxScenarios>,
                                                   IInputBoxService<CategoryInputBoxScenarios>,
                                                   IInputBoxService<NamedItemInputBoxScenarios>,
                                                   IInputBoxService<DocumentInputBoxScenarios>,
                                                   IInputBoxService<SectionInputBoxScenarios>
    {
        public InputBoxResult Show<ReturnType>(InputBoxConfiguration<OrganizerInputBoxScenarios, ReturnType> configuration)
        {
            if (configuration.Scenario == OrganizerInputBoxScenarios.CategoryName)
                return Show(configuration, LocalizedStrings.InputCategoryName);
            else if (configuration.Scenario == OrganizerInputBoxScenarios.DocumentName)
                return Show(configuration, LocalizedStrings.InputDocumentName);
            else if (configuration.Scenario == OrganizerInputBoxScenarios.BookName)
                return Show(configuration, LocalizedStrings.InputBookName); 

            return InputBoxResult.UnsupportedScenario;
        }

        public InputBoxResult Show<ReturnType>(InputBoxConfiguration<CategoryInputBoxScenarios, ReturnType> configuration)
        {
            if (configuration.Scenario == CategoryInputBoxScenarios.CategoryName)
                return Show(configuration, LocalizedStrings.InputCategoryName);
            else if (configuration.Scenario == CategoryInputBoxScenarios.DocumentName)
                return Show(configuration, LocalizedStrings.InputDocumentName);
            else if (configuration.Scenario == CategoryInputBoxScenarios.BookName)
                return Show(configuration, LocalizedStrings.InputBookName);                

            return InputBoxResult.UnsupportedScenario;
        }

        public InputBoxResult Show<ReturnType>(InputBoxConfiguration<NamedItemInputBoxScenarios, ReturnType> configuration)
        {
            if (configuration.Scenario == NamedItemInputBoxScenarios.Rename)
                return Show(configuration, LocalizedStrings.InputNewName);

            return InputBoxResult.UnsupportedScenario;
        }

        public InputBoxResult Show<ReturnType>(InputBoxConfiguration<DocumentInputBoxScenarios, ReturnType> configuration)
        {
            if (configuration.Scenario == DocumentInputBoxScenarios.Tag)
                return Show(configuration, LocalizedStrings.InputTag);

            return InputBoxResult.UnsupportedScenario;
        }

        public InputBoxResult Show<ReturnType>(InputBoxConfiguration<SectionInputBoxScenarios, ReturnType> configuration)
        {
            if (configuration.Scenario == SectionInputBoxScenarios.SetionName)
                return Show(configuration, LocalizedStrings.InputSectonName);

            return InputBoxResult.UnsupportedScenario;
        }

        private InputBoxResult Show<ScenarioType, ReturnType>(InputBoxConfiguration<ScenarioType, ReturnType> config, string prompt) where ScenarioType : System.Enum
        {
            ExclusiveInputBox box = new ()
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Validator = (value, culture) => config.Validator(value, culture),
                InputPrompt = prompt
            };

            if (box.ShowDialog() != true)
                return InputBoxResult.Canceled;
            config.UserInput = config.Converter(box.Text, CultureInfo.CurrentCulture);
            return InputBoxResult.Success;
        }
    }
}
