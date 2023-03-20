using DMOrganizerApp.Views;
using DMOrganizerViewModel;
using System;
using System.Windows;

namespace DMOrganizerApp.Services
{
    public class ReferenceSelector : IReferenceSelector
    {
        public ItemViewModel Select(OrganizerViewModel organizer)
        {
            //open tree window to chose object: if success - set new object by link, if cancel - nothing and no object created 
            ChooseReferenceItem dlg = new ChooseReferenceItem(organizer);
            dlg.Owner = Application.Current.MainWindow;
            bool? res =  dlg.ShowDialog();

            return dlg.SelectedItem ;
            //if (res == true) return; //returns 
            //else return null; //no referenceable will be assigned and no objects created
        }
    }
}
