using CSToolbox;
using CSToolbox.Weak;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using MVVMToolbox.Command;
using MVVMToolbox.Services;
using System;
using System.Collections.ObjectModel;

namespace DMOrganizerViewModel
{
    public enum DocumentInputBoxScenarios
    {
        Tag
    }

    public enum DocumentNotificationScenarios
    {
        DuplicateTag
    }

    public class DocumentNotificationConfiguration : NotificationConfiguration<DocumentNotificationScenarios>
    {
        public string Tag { get; }

        public DocumentNotificationConfiguration(DocumentNotificationScenarios scenario, string tag) : base(scenario)
        {
            Tag = tag;
        }
    }

    public sealed class DocumentViewModel : SectionViewModel
    {
        public ReadOnlyLazyProperty<ObservableCollection<string>> Tags { get; }

        private IDocument Document { get; }

        private IInputBoxService<DocumentInputBoxScenarios> DocumentInputBoxService { get; }
        private INotificationService<DocumentNotificationScenarios> DocumentNotificationService { get; }
        private string? m_TargetTag = null;

        public DeferredCommand AddTag { get; }
        public DeferredCommand<string> DeleteTag { get; }

        public DocumentViewModel(IContext context, IServiceProvider serviceProvider, IDocument document) : base(context, serviceProvider, document)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));

            DocumentInputBoxService = (IInputBoxService<DocumentInputBoxScenarios>)serviceProvider.GetService(typeof(IInputBoxService<DocumentInputBoxScenarios>)) ?? throw new MissingServiceException("Missing InputBoxService.");
            DocumentNotificationService = (INotificationService<DocumentNotificationScenarios>)serviceProvider.GetService(typeof(INotificationService<DocumentNotificationScenarios>)) ?? throw new MissingServiceException("Missing NotificationService.");

            Tags = new ReadOnlyLazyProperty<ObservableCollection<string>>(p =>
            {
                WeakAction<IDocument, DocumentCurrentTagsEventArgs>.CallType handler = (_, e) => Context.Invoke(() =>
                {
                    if (Tags.Value != null)
                        return;
                    p(new ObservableCollection<string>(e.Tags));
                });
                Document.DocumentCurrentTags.Subscribe(handler);
                Document.RequestDocumentCurrentTags();
            });

            Document.DocumentTagsChanged.Subscribe(Document_TagsChanged);

            AddTag = new DeferredCommand(CommandHandler_AddTag, CanExecuteLockingOperation);
            DeleteTag = new DeferredCommand<string>(CommandHandler_DeleteTag, CanExecute_DeleteTag);
        }

        private void CommandHandler_AddTag()
        {
            var config = new InputBoxConfiguration<DocumentInputBoxScenarios, string>(DocumentInputBoxScenarios.Tag, (inV, _) => inV, (inV, _) => NamingRules.IsValidTag(inV) );
            InputBoxResult res = default;
            Context.Invoke(() => res = DocumentInputBoxService.Show(config));

            if (res != InputBoxResult.Success)
                return;
            Context.Invoke(() => LockingOperation = true);
            m_TargetTag = config.UserInput;
            Document.AddDocumentTag(m_TargetTag);
        }

        private bool CanExecute_DeleteTag(string? _) => !LockingOperation;

        private void CommandHandler_DeleteTag(string? tag)
        {
            if (tag == null)
                return;
            Context.Invoke(() => LockingOperation = true);
            m_TargetTag = tag;
            Document.RemoveDocumentTag(m_TargetTag);
        }

        protected override void UpdateCommandsExecutability()
        {
            base.UpdateCommandsExecutability();
            AddTag.InvokeCanExecuteChanged();
            Delete.InvokeCanExecuteChanged();
        }

        private void Document_TagsChanged(IDocument sender, DocumentTagsChangedEventArgs e)
        {
            if (Tags.Value == null)
                return;

            if (m_TargetTag == e.Tag)
            {
                Context.Invoke(() => LockingOperation = false);
                m_TargetTag = null;
                if (e.Type == DocumentTagsChangedEventArgs.ChangeType.TagAdded && e.Result != DocumentTagsChangedEventArgs.ResultType.Success)
                    Context.Invoke(() => DocumentNotificationService.Show(new DocumentNotificationConfiguration(DocumentNotificationScenarios.DuplicateTag, e.Tag)));
            }
                
            
            if (!e.HasChanged)
                return;
            else if (e.Type == DocumentTagsChangedEventArgs.ChangeType.TagAdded)
                Context.Invoke(() => Tags.Value.Add(e.Tag));
            else
                Context.Invoke(() => Tags.Value.Remove(e.Tag));
        }
    }
}
