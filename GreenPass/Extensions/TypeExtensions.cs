using System.Collections.Generic;

namespace GreenPass.Extensions
{
    public static class TypeExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
            => dict.TryGetValue(key, out var value) ? value : default(TValue);
    }
}
