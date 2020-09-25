using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Gommon
{
    /// <summary>
    ///     Extensions for any class in the System namespace, including sub-namespaces, such as System.Text.
    /// </summary>
    public static partial class Extensions
    {
        public static string CalculateUptime(this Process process)
            => (DateTime.Now - process.StartTime).Humanize(3);
        
        public static string ReplaceIgnoreCase(this string str, string toReplace, object replacement)
            => str.Replace(toReplace, replacement.ToString(), StringComparison.OrdinalIgnoreCase);

        public static Task<DiscordMessage> SendFileToAsync(this MemoryStream stream,
            string fileName, DiscordChannel channel, string text = null, bool isTts = false, DiscordEmbed embed = null,
            IEnumerable<IMention> allowedMentions = null)
        {
            return channel.SendFileAsync(fileName, stream, text, isTts, embed, allowedMentions);
        }

        public static bool None<T>(this IEnumerable<T> current, Func<T, bool> predicate) => !current.Any(predicate);
    }
}