using System;
using System.Collections.Generic;

namespace CSToolbox.Weak
{
    /// <summary>
    /// A weak cache which holds weak references to its items, allowing the items to be GC'd when they are not used.
    /// </summary>
    /// <typeparam name="CacheKey">The type of key for lookup</typeparam>
    /// <typeparam name="CacheValue">The type of items to store in this cache</typeparam>
    public class WeakCache<CacheKey, CacheValue> where CacheValue : class where CacheKey : notnull
    {
        protected Dictionary<CacheKey, WeakReference<CacheValue>> CachedValues { get; }
        protected Func<CacheKey, CacheValue> Generator { get; }

        /// <summary>
        /// Creates a weak cache
        /// </summary>
        /// <param name="generator">A method which generates an instance to put into cache based on key</param>
        public WeakCache(Func<CacheKey, CacheValue> generator)
        {
            Generator = generator ?? throw new ArgumentNullException(nameof(generator));
            CachedValues = new();
        }

        /// <summary>
        /// Returns a value from cache or creates it using default generator.
        /// </summary>
        /// <param name="key">The key to get the value for</param>
        /// <returns>Either a value from cache or a newly generated value if non was found</returns>
        public CacheValue this[CacheKey key] => GetValue(key);

        /// <summary>
        /// Returns a value from cache or creates it
        /// </summary>
        /// <param name="key">The key to get the value for</param>
        /// <param name="generator">A generator to create this item if it doesn't exist. Default generator provided in the constructor will be used if one is not provided here.</param>
        /// <returns>Either a value from cache or a newly generated value if non was found</returns>
        public CacheValue GetValue(CacheKey key, Func<CacheKey, CacheValue> generator = null)
        {
            if (CachedValues.TryGetValue(key, out WeakReference<CacheValue> entry) && entry.TryGetTarget(out CacheValue value))
                return value;
            value = (generator ?? Generator)(key);
            CachedValues[key] = new WeakReference<CacheValue>(value);
            return value;
        }
    }
}
