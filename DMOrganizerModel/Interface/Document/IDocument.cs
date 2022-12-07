namespace DMOrganizerModel.Interface.Document
{
    public interface IDocument : ISection
    {
        /// <summary>
        /// Tags of this document
        /// </summary>
        IObservableCollection<string> Tags { get; }
    }
}
