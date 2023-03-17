using System;

namespace CSToolbox
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Checks if this type matches or is dervied from specified type
        /// </summary>
        /// <param name="type">This type</param>
        /// <param name="other">The type to check this type for</param>
        /// <returns>True if this type is dervice or matches the other type</returns>
        public static bool Is(this Type type, Type other)
        {
            return type == other || type?.IsSubclassOf(other) == true;
        }
    }
}
