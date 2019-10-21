using System;

namespace Gommon
{
    public static partial class Extensions
    {
        public static string ReplaceIgnoreCase(this string str, string toReplace, object replacement)
            => str.Replace(toReplace, replacement.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}