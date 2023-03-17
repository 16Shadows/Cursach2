using System.Collections;

namespace CSToolbox
{
    public static class IEnumerableExtensions   
    {
        static public bool Any(this IEnumerable iterator)
        {
            return iterator.GetEnumerator().MoveNext();
        }
    }
}