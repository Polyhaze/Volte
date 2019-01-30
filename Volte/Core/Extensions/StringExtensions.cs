using System;

namespace Volte.Core.Extensions {
    public static class StringExtensions {
        public static bool EqualsIgnoreCase(this string str, string str2) {
            return str.Equals(str2, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}