using CSToolbox;
using CSToolbox.Weak;
using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using System;
using System.Collections.ObjectModel;

namespace DMOrganizerViewModel
{
    public class DocumentViewModel : SectionViewModel
    {
        public ReadOnlyLazyProperty<ObservableCollection<string>> Tags { get; }

        protected IDocument Document { get; }

        public DocumentViewModel(IContext context, IServiceProvider serviceProvider, IDocument document) : base(context, serviceProvider, document)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));

            Tags = new ReadOnlyLazyProperty<ObservableCollection<string>>(p =>
            {
                WeakAction<IDocument, DocumentCurrentTagsEventArgs>.CallType handler = (_, e) => Context.Invoke(() => p.Value = new ObservableCollection<string>(e.Tags));
                Document.DocumentCurrentTags.Subscribe(handler);
                Document.RequestDocumentCurrentTags();
            });
            Document.DocumentTagsChanged.Subscribe(Document_TagsChanged);
        }

        private void Document_TagsChanged(IDocument sender, DocumentTagsChangedEventArgs e)
        {
            if (!e.HasChanged)
                return;
            else if (e.Type == DocumentTagsChangedEventArgs.ChangeType.TagAdded)
                Context.Invoke(() => Tags.Value.Add(e.Tag));
            else
                Context.Invoke(() => Tags.Value.Remove(e.Tag));
        }
    }
}
