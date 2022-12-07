using System.Collections.Generic;

namespace DMOrganizerModel.Interface.Document
{
    public interface IDocument : ISection
    {
        ICollection<string> Tags { get; }
    }
}
