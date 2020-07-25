using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Humanizer;

namespace Gommon
{
    /// <summary>
    ///     Extensions for any class in the System namespace, including sub-namespaces, such as System.Text.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        ///     Appends the given line to the current <see cref="StringBuilder"/> if the given <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="sb">The current <see cref="StringBuilder"/>.</param>
        /// <param name="line">The line to append.</param>
        /// <param name="condition">The condition to test against.</param>
        /// <returns><see cref="StringBuilder"/> with the line appended or not.</returns>
        public static StringBuilder AppendLineIf(this StringBuilder sb, string line, bool condition)
        {
            if (condition)
            {
                sb.AppendLine(line);
            }
            return sb;
        }
        /// <summary>
        ///     Appends the given text to the current <see cref="StringBuilder"/> if the given <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="sb">The current <see cref="StringBuilder"/>.</param>
        /// <param name="text">The text to append.</param>
        /// <param name="condition">The condition to test against.</param>
        /// <returns><see cref="StringBuilder"/> with the current line appended or not.</returns>
        public static StringBuilder AppendIf(this StringBuilder sb, string text, bool condition)
        {
            if (condition)
            {
                sb.Append(text);
            }
            return sb;
        }
        
        public static string ReplaceIgnoreCase(this string str, string toReplace, object replacement)
            => str.Replace(toReplace, replacement.ToString(), StringComparison.OrdinalIgnoreCase);
        
        public static string GetUptime(this Process process)
            => (DateTime.Now - process.StartTime).Humanize(3);
        
        public static string SanitizeParserName(this Type type)
            => type.Name.Replace("Parser", string.Empty);

        public static bool IsMatch(this Regex regex, string str, out Match match)
        {
            var intMatch = regex.Match(str);
            match = intMatch;
            return intMatch.Success;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => enumerable.Count() is 0;
    }
}
