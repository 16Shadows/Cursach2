using MVVMToolbox.ViewModel;
using CSToolbox;
using CSToolbox.Weak;
using DMOrganizerModel.Interface.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMToolbox;
using System.ComponentModel;
using DMOrganizerModel.Interface.References;
using System.Windows.Controls;

namespace DMOrganizerViewModel
{
    internal class ContainerObjectViewModel : ContainerViewModel<IReferenceable>
    {
        // ContainerViewModel already keeps all Object IReferenceable objects, we just need to show them in proper way
        public LazyProperty<IReferenceable> ReferenceableObjectType { get; }
        protected IObject ContainerObject { get; }
        public ContainerObjectViewModel(IContext context, IServiceProvider serviceProvider, IItemContainer<IReferenceable> container, IObject obj) : base(context, serviceProvider, container, obj) { }

        protected override DMOrganizerViewModelBase CreateViewModel(IReferenceable item)
        {
            //type check and creation of different child-objects
            throw new NotImplementedException();
        }
    }
}
