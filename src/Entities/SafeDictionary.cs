using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Gommon;

namespace Volte.Entities
{
    /// <summary>
    ///     Mutable <see cref="Dictionary{TKey,TValue}"/> implementation that solely provides nullable indexing.
    /// </summary>
    /// <typeparam name="TKey">Entry key</typeparam>
    /// <typeparam name="TValue">Entry value</typeparam>
    public class SafeDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TValue : class
    { 
        public SafeDictionary(Dictionary<TKey, TValue> initial = null)
            => (initial ?? new Dictionary<TKey, TValue>()).ForEach(x => Add(x.Key, x.Value));

        public new TValue this[TKey key]
        {
            get => TryGetValue(key, out var value) ? value : null;
            set => base[key] = value;
        }
    }

    public static class SafeDictionaryExt
    {
        public static SafeDictionary<TKey, TValue> AsSafe<TKey, TValue>(this Dictionary<TKey, TValue> current)
            where TValue : class =>
            new SafeDictionary<TKey, TValue>(current ?? new Dictionary<TKey, TValue>());
    }
}