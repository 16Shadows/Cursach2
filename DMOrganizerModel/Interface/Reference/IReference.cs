using System;
using DMOrganizerModel.Interface.Document;

namespace DMOrganizerModel.Interface.Reference
{
    public interface IReference
    {
        ISection Target { get; }
        string EncodeAsString();
    }
}
