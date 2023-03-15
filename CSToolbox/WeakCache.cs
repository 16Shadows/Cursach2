using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSToolbox
{
    /// <summary>
    /// A weak cache which holds weak references to its items, allowing the items to be GC'd when they are not used
    /// </summary>
    /// <typeparam name="CacheKey"></typeparam>
    /// <typeparam name="CacheValue"></typeparam>
    public class WeakCache<CacheKey, CacheValue> where CacheValue : class where CacheKey : notnull
    {
        protected Dictionary<CacheKey, WeakReference<CacheValue>> CachedValues { get; }
        protected Func<CacheKey, CacheValue> Generator { get; }

        /// <summary>
        /// Creates a weak cache
        /// </summary>
        /// <param name="generator">A method which generates an instance to put into cache based on key</param>
        public WeakCache( Func<CacheKey, CacheValue> generator )
        {
            Generator = generator ?? throw new ArgumentNullException(nameof(generator));
            CachedValues = new();
        }

        /// <summary>
        /// See <see cref="GetValue(CacheKey)"/>
        /// </summary>
        public CacheValue this[CacheKey key] => GetValue(key);

        /// <summary>
        /// Returns a value from cache or creates it
        /// </summary>
        /// <param name="key">The key to get the value for</param>
        /// <returns>Either a value from cache or a newly generated value if non was found</returns>
        public CacheValue GetValue(CacheKey key)
        {
            if (CachedValues.TryGetValue(key, out WeakReference<CacheValue> entry) && entry.TryGetTarget(out CacheValue value))
                return value;
            value = Generator(key);
            CachedValues[key] = new WeakReference<CacheValue>(value);
            return value;
        }
    }
}
