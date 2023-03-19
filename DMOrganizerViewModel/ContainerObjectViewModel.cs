﻿using MVVMToolbox.ViewModel;
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
    public class ContainerObjectViewModel : ContainerItemViewModel<IReferenceable>
    {

        // ContainerViewModel already keeps all Object IReferenceable objects, we just need to show them in proper way
        public LazyProperty<IReferenceable> ReferenceableObjectType { get; }
        protected IObject ContainerObject { get; }
        public ContainerObjectViewModel(IContext context, IServiceProvider serviceProvider, IItemContainer<IReferenceable> container, IObject item) : base(context, serviceProvider, container, item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            else ContainerObject = item;
        }
        protected override ItemViewModel CreateViewModel(IReferenceable item)
        {
            if (item is IDocument)
                return new DocumentViewModel(Context, ServiceProvider, item as IDocument);
            else if (item is ISection)
                return new SectionViewModel(Context, ServiceProvider, item as ISection);
            else throw new ArgumentException("Unsupported Referencable type");
            //type check and creation of different child-objects
            //throw new NotImplementedException();
        }
    }
}
