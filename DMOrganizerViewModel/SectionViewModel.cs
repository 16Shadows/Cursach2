using CSToolbox;
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

            Section.SectionContentChanged.Subscribe(Section_ContentChanged);

            Content = new LazyProperty<string>( _ => Section.RequestSectionContentUpdate() );
        }

        private void Section_ContentChanged(ISection sender, SectionContentChangedEventArgs e)
        {
            Context.Invoke(() => Content.Value = e.Content);
        }

        protected override ItemViewModel CreateViewModel(ISection item) => new SectionViewModel(Context, ServiceProvider, item);
    }
}
