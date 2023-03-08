using DMOrganizerModel.Interface.Items;
using System;
using System.Collections.Generic;

namespace DMOrganizerModel.Interface.Organizer
{
    public class OrganizerCurrentContentEventArgs : EventArgs
    {
        public List<IItem> Content { get; }

        public OrganizerCurrentContentEventArgs(List<IItem> content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }

    public class OrganizerContentChangedEventArgs : EventArgs
    {
        public enum ChangeType
        {
            ItemAdded,
            ItemRemoved
        }

        public IItem Item { get; }
        public ChangeType Type { get; }

        public OrganizerContentChangedEventArgs(IItem item, ChangeType type)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
            Type = type;
        }
    }

    public interface IOrganizer
    {
        /// <summary>
        /// Is invoked when a request for this organizer's content is complete
        /// </summary>
        event TypedEventHandler<IOrganizer, OrganizerCurrentContentEventArgs> OrganizerCurrentContent;

        /// <summary>
        /// Is invoked when the content of this organizer changes
        /// </summary>
        event TypedEventHandler<IOrganizer, OrganizerContentChangedEventArgs> OrganizerContentChanged;


        /// <summary>
        /// Requests organizer's current content
        /// </summary>
        void RequestCurrentOrganizerContent();

        /// <summary>
        /// Adds an item to the organizer
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <exception cref="ArgumentException">Can be throw if the IOrganizer already has the item</exception>
        void AddOrganizerItem(IItem item);

        /// <summary>
        /// Removes an item from the organizer
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <exception cref="ArgumentException">Can be throw if the IOrganizer doesn't have the item</exception>
        void RemoveOrganizerItem(IItem item);

        /// <summary>
        /// Creates a category in the organizer
        /// </summary>
        /// <param name="name">The name of the category</param>
        /// <exception cref="ArgumentException">Can be throw if the IOrganizer already contains a category with the same name</exception>
        void CreateCategory(string name);

        /// <summary>
        /// Creates a document in the organizer
        /// </summary>
        /// <param name="name">The name of the category</param>
        /// <exception cref="ArgumentException">Can be throw if the IOrganizer already contains a document with the same name</exception>
        void CreateDocument(string name);
    }
}
