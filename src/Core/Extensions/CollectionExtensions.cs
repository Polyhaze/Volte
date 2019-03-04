using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Volte.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static Stream ToStream(this IEnumerable<byte> bytes)
        {
            var memStream = new MemoryStream(bytes as byte[] ?? bytes.ToArray(), false);
            memStream.Seek(0, SeekOrigin.Begin);
            return memStream;
        }

        public static bool ContainsIgnoreCase(this IEnumerable<string> strings, string element) =>
            strings.Contains(element, StringComparer.OrdinalIgnoreCase);

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> coll, Func<T, TKey> selector) =>
            coll.GroupBy(selector).Select(x => x.FirstOrDefault());
    }
}