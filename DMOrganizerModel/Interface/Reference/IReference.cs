namespace DMOrganizerModel.Interface.Reference
{
    public interface IReference
    {
        /// <summary>
        /// The item this instance references
        /// </summary>
        IReferenceable Item { get; }

        /// <summary>
        /// Encodes this reference in a string format
        /// </summary>
        /// <returns>String-encoded reference</returns>
        string Encode();
    }
}
