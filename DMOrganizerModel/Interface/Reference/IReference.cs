using DMOrganizerModel.Interface.Document;

namespace DMOrganizerModel.Interface.Reference
{
    public interface IReference
    {
        /// <summary>
        /// The entity this reference refers to
        /// </summary>
        ISection Target { get; }

        /// <summary>
        /// Converts this reference to a uniquely-revertable string
        /// </summary>
        /// <returns>The unqiuely-revertable string-encoded reference</returns>
        string EncodeAsString();
    }
}
