using System;
using System.Diagnostics;
using System.IO;
using System.Text;
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

        public static string Prepend(this string str, string other) => str.Insert(0, other);

        public static string Repeat(this string str, int times)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < times; i++)
            {
                sb.Append(str);
            }

            return sb.ToString();
        }
        
        public static string CalculateUptime(this Process process)
            => (DateTime.Now - process.StartTime).Humanize(3);

        public static Task<IUserMessage> SendFileToAsync(this MemoryStream stream, 
            ITextChannel channel, string filename, string text = null, bool isTts = false, Embed embed = null, RequestOptions options = null,
            bool isSpoiler = false, AllowedMentions allowedMentions = null, MessageReference reference = null)
        {
            return channel.SendFileAsync(stream, filename, text, isTts, embed, options, isSpoiler, allowedMentions, reference);
        }

        public static string FormatBoldString(this DateTime dt)
        {
            var res = dt.FormatPrettyString().Split(" ");
            res[1] = Format.Bold(res[1]);
            res[2] = $"{Format.Bold(res[2].TrimEnd(','))},";
            res[4] = Format.Bold(res[4]);
            return res.Join(" ");
        }
        
        public static string FormatBoldString(this DateTimeOffset dt)
        {
            return dt.DateTime.FormatBoldString();
        }
    }
}
