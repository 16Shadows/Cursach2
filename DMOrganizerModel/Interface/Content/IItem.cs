using DMOrganizerModel.Interface.NavigationTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Interface.Content
{
    public interface IItem
    {
        /// <summary>
        /// The title of this section
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Renames this section
        /// </summary>
        /// <param name="name">New title of this section</param>
        /// <returns>True if the request has been successfully queued, false otherwise</returns>
        Task Rename(string name);

        /// <summary>
        /// Called when renaming has been complete
        /// </summary>
        event OperationResultEventHandler<IItem>? Renamed;
    }
}
