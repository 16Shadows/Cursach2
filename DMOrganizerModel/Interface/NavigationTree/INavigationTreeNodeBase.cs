using System.ComponentModel;
using System.Threading.Tasks;

namespace DMOrganizerModel.Interface.NavigationTree
{
    public interface INavigationTreeNodeBase : INotifyPropertyChanged
    {
        /// <summary>
        /// The name of this node
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The parent of this node
        /// </summary>
        INavigationTreeRoot Parent { get; }

        /// <summary>
        /// Renames this node
        /// </summary>
        /// <param name="name">New title of this node</param>
        /// <returns>True if the request has been successfully queued, false otherwise</returns>
        Task Rename(string name);

        /// <summary>
        /// Called when renaming has been complete
        /// </summary>
        event OperationResultEventHandler<INavigationTreeNodeBase>? Renamed;

        /// <summary>
        /// Changes parent of this node
        /// </summary>
        /// <param name="newParent">New parent of this node</param>
        /// <returns>True if the request has been successfully queued, false otherwise</returns>
        Task ChangeParent(INavigationTreeRoot newParent);

        /// <summary>
        /// Called when renaming has been complete
        /// </summary>
        event OperationResultEventHandler<INavigationTreeNodeBase>? ParentChanged;
    }
}
