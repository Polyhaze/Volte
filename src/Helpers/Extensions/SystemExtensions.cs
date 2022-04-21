﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public static bool TryGetValue<TKey, TValue, TNewValue>(this Dictionary<TKey, TValue> dict, TKey key, out TNewValue value) where TValue : class
        {
            value = Lambda.TryOrNull(() => dict[key]).Cast<TNewValue>();
            
            return value != null;
        }
        
        public static Task Then(this Task task, Func<Task> continuation) 
            => task.ContinueWith(async _ => await continuation());

        public static async Task<T> Then<T>(this Task<T> task, Func<T, Task> continuation)
        {
            var result = await task;
            await continuation(result);
            return result;
        }

        public static IEnumerable<TSource> WhereNotNull<TSource>(this IEnumerable<TSource> enumerable) => enumerable.Where(x => x != null);

        public static async Task Then<T>(this ValueTask<T> task, Func<T, Task> continuation) => await continuation(await task);
        
        public static bool ContainsAnyIgnoreCase(this string str, params string[] possibleContents) 
            => possibleContents.Any(str.ContainsIgnoreCase);
        
        public static bool ExistsInAny<T>(this T @this, params IEnumerable<T>[] collections) 
            => collections.Any(x => x.Contains(@this));

        public static string ReplaceIgnoreCase(this string str, string toReplace, object replacement)
            => str.Replace(toReplace, replacement.ToString(), StringComparison.OrdinalIgnoreCase);

        public static string CalculateUptime(this Process process)
            => (DateTime.Now - process.StartTime).Humanize(3);

        public static Task<IUserMessage> SendFileToAsync(this MemoryStream stream,
            ITextChannel channel, string filename, string text = null, bool isTts = false, Embed embed = null,
            RequestOptions options = null,
            bool isSpoiler = false, AllowedMentions allowedMentions = null, MessageReference reference = null)
            => channel.SendFileAsync(stream, filename, text, isTts, embed, options, isSpoiler, allowedMentions,
                reference);

        public static Exception GetInnermostException(this Exception exception)
        {
            var current = exception.InnerException;
            while (current != null)
            {
                if (current?.InnerException is null)
                    break;
                current = current.InnerException;
            }
            return current ?? exception;
        }

        public static string FormatBoldString(this DateTime dt)
            => dt.FormatPrettyString().Split(" ").Apply(arr =>
            {
                arr[1] = Format.Bold(arr[1]);
                arr[2] = $"{Format.Bold(arr[2].TrimEnd(','))},";
                arr[4] = Format.Bold(arr[4]);
            }).Join(" ");

        public static string FormatBoldString(this DateTimeOffset dt) 
            => dt.DateTime.FormatBoldString();
    }
}