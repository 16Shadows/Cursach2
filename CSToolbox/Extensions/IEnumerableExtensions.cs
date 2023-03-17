using System.Collections;

namespace CSToolbox.Extensions
{
    public static class IEnumerableExtensions
    {
        static public bool Any(this IEnumerable iterator)
        {
            return iterator.GetEnumerator().MoveNext();
        }
    }
}