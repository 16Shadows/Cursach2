using DMOrganizerModel.Implementation.Organizers;
using DMOrganizerModel.Interface.Organizer;
using DMOrganizerViewModel;
using Microsoft.Win32;
using MVVMToolbox;
using MVVMToolbox.Command;
using MVVMToolbox.Services;
using MVVMToolbox.ViewModel;
using System;
using System.Windows;

namespace DMOrganizerApp.ViewModels
{
    public enum MainViewModelNotificationScenarios
    {
        FailedToOpenOrganizer
    }

    public class MainViewModelNotificationConfiguration : NotificationConfiguration<MainViewModelNotificationScenarios>
    {
        public string Error { get; }

        public MainViewModelNotificationConfiguration(MainViewModelNotificationScenarios scenario, string error) : base(scenario)
        {
            Error = error;
        }
    }

    internal class MainViewModel : ViewModelHost
    {
        private string m_ActiveOrganizerName = "";
        public string ActiveOrganizerName
        {
            get => m_ActiveOrganizerName;
            set
            {
                if (m_ActiveOrganizerName == value)
                    return;

                m_ActiveOrganizerName = value;
                InvokePropertyChanged(nameof(ActiveOrganizerName));
            }
        }

        public DeferredCommand OpenOrganizer { get; }
        public DeferredCommand CreateOrganizer { get; }

        public MainViewModel(IContext context, IServiceProvider serviceProvider) : base(context, serviceProvider, null)
        {
            OpenOrganizer = new DeferredCommand(CommandHandler_OpenOrganizer);
            CreateOrganizer = new DeferredCommand(CommandHandler_CreateOrganizer);
        }

        private void CommandHandler_CreateOrganizer()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                AddExtension = true,
                CheckPathExists = true,
                DereferenceLinks = true,
                OverwritePrompt = true,
                Filter = "DMOrganizer files (*.dmo)|*.dmo"
            };
            if (saveFileDialog.ShowDialog() == false)
                return;
            OrganizersStorageModel.DeleteOrganizer(saveFileDialog.FileName);
            try
            {
                IOrganizer org = OrganizersStorageModel.LoadOrganizer(saveFileDialog.FileName);
                Context.Invoke(() =>
                {
                    ActiveViewModel = new OrganizerViewModel(Context, ServiceProvider, org);
                    ActiveOrganizerName = saveFileDialog.FileName;
                });
            }
            catch (Exception e)
            {
                MessageBox.Show( "Something went wrong:" + Environment.NewLine + e.ToString() );
                return;
            }
        }

        private void CommandHandler_OpenOrganizer()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DereferenceLinks = true,
                Filter = "DMOrganizer files (*.dmo)|*.dmo"
            };
            if (openFileDialog.ShowDialog() == false)
                return;
            try
            {
                IOrganizer org = OrganizersStorageModel.LoadOrganizer(openFileDialog.FileName);
                Context.Invoke(() =>
                {
                    ActiveViewModel = new OrganizerViewModel(Context, ServiceProvider, org);
                    ActiveOrganizerName = openFileDialog.FileName;
                });
            }
            catch (Exception e)
            {
                MessageBox.Show( "Something went wrong:" + Environment.NewLine + e.ToString() );
                return;
            }
        }
    }
}
