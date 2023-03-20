using System;
using System.Collections.Generic;

namespace CSToolbox.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Removes first occurence of a matching item in the list
        /// </summary>
        /// <typeparam name="T">The type of items store in the list</typeparam>
        /// <param name="list">The list</param>
        /// <param name="predicate">A predicate to determine a matching item</param>
        public static void Remove<T>(this IList<T> list, Predicate<T> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (!predicate(list[i]))
                    continue;
                list.RemoveAt(i);
                return;
            }
        }

        public static int LastIndexOf<T>(this IList<T> list, Predicate<T> predicate)
        {
            for (int i = list.Count-1; i >= 0; i--)
            {
                if (!predicate(list[i]))
                    continue;
                return i;
            }
            return -1;
        }
    }
}
