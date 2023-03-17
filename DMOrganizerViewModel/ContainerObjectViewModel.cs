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

namespace DMOrganizerViewModel
{
    internal class ContainerObjectViewModel : DMOrganizerViewModelBase
    {
        //has INotifyPropertyChanged, method void InvokePropertyChanged(string name)
        public ContainerObjectViewModel(IContext context, IServiceProvider serviceProvider) : base(context, serviceProvider) { }
    }
}
