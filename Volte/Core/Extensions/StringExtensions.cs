using System;
using Discord;
using Discord.Commands;

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
    }
}