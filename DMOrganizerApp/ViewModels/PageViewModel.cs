using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerApp.ViewModels
{
    internal class PageViewModel
    {

        // moved from model, need to think about it and how to do
        public class PageEditModeChangedEventArgs : EventArgs
        {
            public bool EditMode { get; }
            public int Position { get; }
            public PageEditModeChangedEventArgs(int pos, bool mode)
            {
                EditMode = mode;
                Position = pos;
            }
        }
        //  editMode - true/false
        event TypedEventHandler<IPage, PageEditModeChangedEventArgs> PageEditModeChanged;

        //Will enable or disable edit functions on page(move containers,
        //add containers, edit container content), change mode to read pages and edit
        // Need to set "false" when current page is changed 
        bool EditMode { get; set; }

        //Need PageEditModeChanged
        /// <summary>
        /// Sets page's edit mode.
        /// </summary>
        /// <param name="newEditMode"></param>
        void SetEditMode(bool newEditMode) //event what mode now
        {
            EditMode = newEditMode;
        }
    }
}
