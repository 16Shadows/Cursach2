using DMOrganizerModel.Interface.References;
using CSToolbox.Weak;
using System;

public class ObjectUpdateLinkEventArgs : EventArgs
{
    public enum ResultType
    {
        /// <summary>
        /// Succesful update, size is correct
        /// </summary>
        Success,

        /// <summary>
        /// Updation failure, incorrect size
        /// </summary>
        IncorrectLink
    }

    public ResultType Result { get; }
    public string Link { get; }
    public ObjectUpdateLinkEventArgs(string link, ResultType result)
    {
        Link = link;
        Result = result;
    }
}
namespace DMOrganizerModel.Interface.Items
{
    /// <summary>
    /// Object for containers on pages, can contain links to documents/sections/other linkeable things.
    /// </summary>
    public interface IObject: IItem, IItemContainer<IReferenceable>
    {
        // link to object
        // request link (IItemContainer)
        // check if all child-objects are valid and can be displayed            !!!
        // (if link to item is not valid or item was deleted - show error) 

        WeakEvent<IObject, ObjectUpdateLinkEventArgs> ObjectUpdateLink { get; }

        /// <summary>
        /// Sets content to object with new link.
        /// </summary>
        /// <param name="newLink">Link to object that will be displayed and stored.</param>
        void UpdateContent(IReference newLink); //event iitemcontainer
        void SetLink(IReference link);
    }
}
