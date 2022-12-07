using DMOrganizerModel.Interface.Document;
using System;
using System.Collections.Generic;

namespace DMOrganizerModel.Implementation.Document
{
    internal class Document : Section, IDocument
    {
        public ICollection<string> Tags => throw new NotImplementedException();
    }
}
