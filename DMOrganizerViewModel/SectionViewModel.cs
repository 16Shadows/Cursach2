using CSToolbox;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using MVVMToolbox.Command;
using MVVMToolbox.Services;
using System;

namespace DMOrganizerViewModel
{
    public enum SectionInputBoxScenarios
    {
        SetionName
    }

    public enum SectionNotificationScenarios
    {
        DuplicateName
    }

    public class SectionNotificationConfiguration : NotificationConfiguration<SectionNotificationScenarios>
    {
        public string Name { get; }

        public SectionNotificationConfiguration(SectionNotificationScenarios scenario, string name) : base(scenario)
        {
            Name = name;
        }
    }

    public class SectionViewModel : NamedContainerViewModel<ISection>
    {
        public LazyProperty<string> Content { get; }

        protected ISection Section { get; }
        protected IInputBoxService<SectionInputBoxScenarios> SectionInputBoxService { get; }
        protected INotificationService<SectionNotificationScenarios> SectionNotificationService { get; }

        public DeferredCommand CreateSection { get; protected init; }
        private string? m_CreatingSection = null;
        private bool m_SettingContent;
        private bool m_RecursionGuard;

        public SectionViewModel(IContext context, IServiceProvider serviceProvider, ISection section) : base(context, serviceProvider, section, section)
        {
            Section = section ?? throw new ArgumentNullException(nameof(section));

            SectionInputBoxService = (IInputBoxService<SectionInputBoxScenarios>)serviceProvider.GetService(typeof(IInputBoxService<SectionInputBoxScenarios>)) ?? throw new MissingServiceException("Missing InputBoxService.");
            SectionNotificationService = (INotificationService<SectionNotificationScenarios>)serviceProvider.GetService(typeof(INotificationService<SectionNotificationScenarios>)) ?? throw new MissingServiceException("Missing InputBoxService.");

            Content = new LazyProperty<string>( _ => Section.RequestSectionContentUpdate() );

            Section.SectionContentChanged.Subscribe(Section_ContentChanged);
            Section.SectionItemCreated.Subscribe(Section_ItemCreated);
            Content.WeakPropertyChanged.Subscribe(Content_PropertyChanged);

            CreateSection = new DeferredCommand(CommandHandler_CreateSection, CanExecuteLockingOperation);
        }

        private void CommandHandler_CreateSection()
        {
            var config = new InputBoxConfiguration<SectionInputBoxScenarios, string>(SectionInputBoxScenarios.SetionName, (inV, _) => inV, (inV, _) => NamingRules.IsValidName(inV) );
            InputBoxResult res = default;
            Context.Invoke(() => res = SectionInputBoxService.Show(config));

            if (res != InputBoxResult.Success)
                return;
            Context.Invoke(() => LockingOperation = true);
            m_CreatingSection = config.UserInput;
            Section.CreateSection(m_CreatingSection);
        }

        private void Section_ItemCreated(ISection section, SectionItemCreatedEventArgs e)
        {
            if (!LockingOperation || m_CreatingSection != e.Name)
                return;

            if (e.Result == SectionItemCreatedEventArgs.ResultType.DuplicateName)
                Context.Invoke(() => SectionNotificationService.Show(new SectionNotificationConfiguration(SectionNotificationScenarios.DuplicateName, e.Name)));

            m_CreatingSection = null;
            Context.Invoke(() => LockingOperation = false);
        }

        private void Content_PropertyChanged(LazyProperty<string> _)
        {
            if (m_RecursionGuard)
                return;

            Context.Invoke(() => LockingOperation = true);
            m_SettingContent = true;
            Section.UpdateContent(Content.Value);
        }

        private void Section_ContentChanged(ISection sender, SectionContentChangedEventArgs e)
        {
            Context.Invoke(() =>
            {
                m_RecursionGuard = true;
                Content.Value = e.Content;
                m_RecursionGuard = false;
                if (LockingOperation && m_SettingContent)
                    LockingOperation = false;
            });
        }

        protected override ItemViewModel CreateViewModel(ISection item) => new SectionViewModel(Context, ServiceProvider, item);
    }
}
