using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Humanizer;

namespace Gommon
{
    /// <summary>
    ///     Extensions for any class in the System namespace, including sub-namespaces, such as System.Text.
    /// </summary>
    public static partial class Extensions
    {
        public static string ReplaceIgnoreCase(this string str, string toReplace, object replacement)
            => str.Replace(toReplace, replacement.ToString(), StringComparison.OrdinalIgnoreCase);
        
        public static string CalculateUptime(this Process process)
            => (DateTime.Now - process.StartTime).Humanize(3);

        public static Task<IUserMessage> SendFileToAsync(this MemoryStream stream, 
            ITextChannel channel, string filename, string text = null, bool isTts = false, Embed embed = null, RequestOptions options = null,
            bool isSpoiler = false, AllowedMentions allowedMentions = null, MessageReference reference = null)
        {
            return channel.SendFileAsync(stream, filename, text, isTts, embed, options, isSpoiler, allowedMentions, reference);
        }
    }
}

namespace System.Linq
{
    public static class Extensions
    {
        public static bool AnyGet<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate,
            out TSource value)
        {
            source = source.ToArray();
            if (source.Any(predicate))
            {
                value = source.FirstOrDefault(predicate);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }
}
