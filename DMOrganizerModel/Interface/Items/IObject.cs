using CSToolbox.Weak;
using DMOrganizerModel.Interface.References;
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
public class ObjectCurrentContentEventArgs : EventArgs
{
    public string Link {get;}
    public ObjectCurrentContentEventArgs(string link)
    {
        Link = link;
    }
}
namespace DMOrganizerModel.Interface.Items
{
    /// <summary>
    /// Object for containers on pages, can contain links to documents/sections/other linkeable things.
    /// </summary>
    public interface IObject: IItem
    {
        // link to object
        // request link (IItemContainer)

        WeakEvent<IObject, ObjectUpdateLinkEventArgs> ObjectUpdateLink { get; }
        WeakEvent<IObject, ObjectCurrentContentEventArgs> ObjectCurrentContent { get; }

        /// <summary>
        /// Sets content to object with new link.
        /// </summary>
        /// <param name="newLink">Link to object that will be displayed and stored.</param>
        void UpdateContent(IReference newLink); //event iitemcontainer
        void SetLink(IReference link);
        string GetObjectLink();
        IReference GetReferenceByLink(string link);
    }
}
