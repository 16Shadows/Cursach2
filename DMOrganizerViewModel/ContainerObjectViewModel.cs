using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Interface.Organizer;
using DMOrganizerModel.Interface.References;
using MVVMToolbox;
using MVVMToolbox.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerViewModel
{
    public class ContainerObjectViewModel : ItemViewModel
    {
        //to change opened referenceable views in object view
        private ViewModelBase? m_ActivePageViewModel;
        public ViewModelBase? ActivePageViewModel
        {
            get => m_ActivePageViewModel;
            set
            {
                if (m_ActivePageViewModel == value)
                    return;
                ViewModelBase? oldvm = m_ActivePageViewModel;
                if (oldvm != null)
                    Context.BeginInvoke(() => oldvm.Unload());
                m_ActivePageViewModel = value;
                m_ActivePageViewModel?.Load();
                InvokePropertyChanged(nameof(ActivePageViewModel));
            }
        }
        protected IObject ContainerObject { get; }
        public ContainerObjectViewModel(IContext context, IServiceProvider serviceProvider, IObject item, OrganizerViewModel org) : base(context, serviceProvider, item, org)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            else ContainerObject = item;

            ContainerObject.ObjectUpdateLink.Subscribe(ContainerObject_LinkUpdate);
            ContainerObject.ObjectCurrentContent.Subscribe(ContainerObject_CurrentContent);
            ContainerObject.GetObjectLink();
        }
        public void ContainerObject_CurrentContent(IObject sender, ObjectCurrentContentEventArgs e)
        {
            IReference reference = ContainerObject.GetReferenceByLink(e.Link);
            IReferenceable item = reference.Item;

            if (item is IDocument)
            {
                DocumentViewModel doc = new DocumentViewModel(Context, ServiceProvider, item as IDocument, OrganizerReference.Target as OrganizerViewModel);
                ActivePageViewModel = doc;
            }
            else if (item is ISection)
            {
                SectionViewModel sec = new SectionViewModel(Context, ServiceProvider, item as IDocument, OrganizerReference.Target as OrganizerViewModel);
                ActivePageViewModel = sec;
            }
            else throw new InvalidOperationException("Unsupported object type for object.");
        }

        public void ContainerObject_LinkUpdate(IObject sender, ObjectUpdateLinkEventArgs e)
        {
            if (e.Result == ObjectUpdateLinkEventArgs.ResultType.Success)
            {
                IReference reference = ContainerObject.GetReferenceByLink(e.Link);
                IReferenceable item = reference.Item;

                if (item is IDocument)
                {
                    DocumentViewModel doc = new DocumentViewModel(Context, ServiceProvider, item as IDocument, OrganizerReference.Target as OrganizerViewModel);
                    ActivePageViewModel = doc;
                }
                else if (item is ISection)
                {
                    SectionViewModel sec = new SectionViewModel(Context, ServiceProvider, item as IDocument, OrganizerReference.Target as OrganizerViewModel);
                    ActivePageViewModel = sec;
                }
                else throw new InvalidOperationException("Unsupported object type for object.");
            }
                
            else return;
        }
    }
}
