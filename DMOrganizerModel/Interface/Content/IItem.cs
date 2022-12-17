using System.ComponentModel;
using System.Threading.Tasks;

namespace DMOrganizerModel.Interface.Content
{
    public interface IItem : INotifyPropertyChanged
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
