using DMOrganizerModel.Interface.References;

namespace DMOrganizerModel.Interface.Items
{
    /// <summary>
    /// Object for containers on pages, can contain links to documents/sections/other linkeable things.
    /// </summary>
    public interface IObject: IItem, IItemContainer<IReferenceable>
    {
        // link to object
        // request link (IItemContainer)

        void UpdateContent(IReferenceable newLink); //event iitemcontainer
    }
}
