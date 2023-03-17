using DMOrganizerApp.UserControls;
using DMOrganizerViewModel;
using MVVMToolbox.Services;
using System.Globalization;
using System.Windows;

namespace DMOrganizerApp.Services
{
    internal class DMOrganizerAppInputBoxService : IInputBoxService<OrganizerInputBoxScenarios>
    {
        public InputBoxResult Show<ReturnType>(InputBoxConfiguration<OrganizerInputBoxScenarios, ReturnType> configuration)
        {
            ExclusiveInputBox box = new ()
            {
                InputPrompt = "Test",
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Validator = (value, culture) => configuration.Validator(value, culture)
            };
            if (box.ShowDialog() != true)
                return InputBoxResult.Canceled;
            configuration.UserInput = configuration.Converter(box.Text, CultureInfo.CurrentCulture);
            return InputBoxResult.Success;
        }
    }
}
