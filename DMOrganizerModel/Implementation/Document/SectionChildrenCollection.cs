using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Document;

namespace DMOrganizerModel.Implementation.Document
{
    internal class SectionChildrenCollection : IObservableList<ISection>
    {
        public ISection this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        ISection IReadOnlyList<ISection>.this[int index] => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public void Add(ISection item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(ISection item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(ISection[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<ISection> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(ISection item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, ISection item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(ISection item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
