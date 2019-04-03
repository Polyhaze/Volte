using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Volte.Extensions
{
    public static class CollectionExtensions
    {
        public static Stream ToStream(this IEnumerable<byte> bytes)
            => new MemoryStream(bytes.Cast<byte[]>() ?? bytes.ToArray(), false) {Position = 0};

        public static bool ContainsIgnoreCase(this IEnumerable<string> strings, string element)
            => strings.Contains(element, StringComparer.OrdinalIgnoreCase);

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> coll, Func<T, TKey> selector)
            => coll.GroupBy(selector).Select(x => x.FirstOrDefault());

        public static string Random(this string[] arr)
        {
            var r = new Random();
            return arr[r.Next(0, arr.Length)];
        }
    }
}