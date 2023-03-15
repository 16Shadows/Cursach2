using CSToolbox;
using CSToolbox.Weak;
using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using System;

namespace DMOrganizerViewModel
{
    public class SectionViewModel : NamedContainerViewModel<ISection>
    {
        public LazyProperty<string> Content { get; }

        protected ISection Section { get; }

        public SectionViewModel(IContext context, IServiceProvider serviceProvider, ISection section) : base(context, serviceProvider, section, section)
        {
            Section = section ?? throw new ArgumentNullException(nameof(section));

            Content = new LazyProperty<string>(p =>
            {
                WeakAction<ISection, SectionContentChangedEventArgs>.CallType handler = (_, e) => Context.Invoke(() => Content.Value = e.Content);
                Section.SectionContentChanged.Subscribe(handler);
                Section.RequestSectionContentUpdate();
            });

            Section.SectionContentChanged.Subscribe(Section_ContentChanged);
        }

        private void Section_ContentChanged(ISection sender, SectionContentChangedEventArgs e)
        {
            if (!e.HasChanged)
                return;
            Context.Invoke(() => Content.Value = e.Content);
        }

        protected override ViewModelBase CreateViewModel(ISection item) => new SectionViewModel(Context, ServiceProvider, item);
    }
}
