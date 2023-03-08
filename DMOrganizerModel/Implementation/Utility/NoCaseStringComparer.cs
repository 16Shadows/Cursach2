using System.Collections.Generic;

namespace DMOrganizerModel.Implementation.Utility
{
    internal class NoCaseStringComparer : IEqualityComparer<string>
    {
        public bool Equals(string? x, string? y)
        {
            return string.Compare(x, y, true) == 0;
        }

        public int GetHashCode(string obj)
        {
            return obj.ToLower().GetHashCode();
        }
    }
}
