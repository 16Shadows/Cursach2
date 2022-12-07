using DMOrganizerModel.Interface.Document;
using System;
using System.Collections.Generic;

namespace DMOrganizerModel.Implementation.Document
{
    internal class Section : ISection
    {
        public string Title { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Content { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<ISection> Children => throw new NotImplementedException();
    }
}
