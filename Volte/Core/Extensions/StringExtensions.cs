using System;
using System.Collections.Generic;

namespace Volte.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string str, string str2)
        {
            return str.Equals(str2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this string str, string str2)
        {
            return str.Contains(str2, StringComparison.OrdinalIgnoreCase);
        }

        public static int[] GetUnicodePoints(this string emoji)
        {
            var pts = new List<int>(emoji.Length);
            for (var i = 0; i < emoji.Length; i++)
            {
                var pt = char.ConvertToUtf32(emoji, i);
                if (pt != 0xFE0F)
                    pts.Add(pt);
                if (char.IsHighSurrogate(emoji[i]))
                    i++;
            }

            return pts.ToArray();
        }
    }
}