using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Document;
using System;

namespace DMOrganizerModel.Implementation.Document
{
    internal class Document : Section, IDocument
    {
        public IObservableCollection<string> Tags => throw new NotImplementedException();
    }
}
