using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DMOrganizerModel.Implementation
{
    internal class NoCaseStringComparer : IEqualityComparer<string>
    {
        public bool Equals(string? x, string? y)
        {
            return string.Compare(x, y, true) == 0;
        }

        public int GetHashCode([DisallowNull] string obj)
        {
            return obj.ToLower().GetHashCode();
        }
    }
}
