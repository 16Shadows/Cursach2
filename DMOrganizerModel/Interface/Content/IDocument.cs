namespace DMOrganizerModel.Interface.Content
{
    public interface IDocument : ISection
    {
        /// <summary>
        /// Tags of this document
        /// </summary>
        IObservableCollection<string> Tags { get; }
    }
}
