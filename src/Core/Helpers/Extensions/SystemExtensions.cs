using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
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
        public static bool EqualsAnyIgnoreCase(this string str, params string[] potentialMatches) 
            => potentialMatches.Any(str.EqualsIgnoreCase);

        public static string ReplaceIgnoreCase(this string str, string toReplace, object replacement)
            => str.Replace(toReplace, replacement.ToString(), StringComparison.OrdinalIgnoreCase);

        public static string Prepend(this string str, string other) => str.Insert(0, other);

        public static void For(this int timesToLoop, Action action)
        {
            for (var i = 0; i < timesToLoop; i++)
                action();
        }

        public static string Repeat(this string str, int times) 
            => new StringBuilder().Apply(sb => times.For(() => sb.Append(str))).ToString();

        public static string CalculateUptime(this Process process)
            => (DateTime.Now - process.StartTime).Humanize(3);

        public static Task<IUserMessage> SendFileToAsync(this MemoryStream stream,
            ITextChannel channel, string filename, string text = null, bool isTts = false, Embed embed = null,
            RequestOptions options = null,
            bool isSpoiler = false, AllowedMentions allowedMentions = null, MessageReference reference = null)
            => channel.SendFileAsync(stream, filename, text, isTts, embed, options, isSpoiler, allowedMentions,
                reference);

        public static string FormatBoldString(this DateTime dt)
            => dt.FormatPrettyString().Split(" ").Apply(arr =>
            {
                arr[1] = Format.Bold(arr[1]);
                arr[2] = $"{Format.Bold(arr[2].TrimEnd(','))},";
                arr[4] = Format.Bold(arr[4]);
            }).Join(" ");

        public static string FormatBoldString(this DateTimeOffset dt) 
            => dt.DateTime.FormatBoldString();


        public static T Apply<T>(this T curr, Action<T> apply)
        {
            apply(curr);
            return curr;
        }
    }
}