using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Utility
{
    /// <summary>
    /// Is thrown when an attempt is made to add an IItem which is already a parent of IContainerItem to another IContainerItem
    /// </summary>
    public sealed class MultipleParentException : Exception
    {
        public MultipleParentException() : base() {}
        public MultipleParentException(string message) : base(message) {}
    }
}
