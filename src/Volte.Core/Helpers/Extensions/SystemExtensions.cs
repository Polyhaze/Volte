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
        private const int MemoryTierSize = 1024;

        public static string ReplaceIgnoreCase(this string str, string toReplace, object replacement)
            => str.Replace(toReplace, replacement.ToString(), StringComparison.OrdinalIgnoreCase);

        public static string CalculateUptime(this Process process)
            => (DateTime.Now - process.StartTime).Humanize(3);

        public static string GetMemoryUsage(this Process process, MemoryType memType)
        {
            var res = process.PrivateMemorySize64;
            return memType switch
            {
                MemoryType.Terabytes => $"{res / MemoryTierSize / MemoryTierSize / MemoryTierSize / MemoryTierSize} TB",
                MemoryType.Gigabytes => $"{res / MemoryTierSize / MemoryTierSize / MemoryTierSize} GB",
                MemoryType.Megabytes => $"{res / MemoryTierSize / MemoryTierSize} MB",
                MemoryType.Kilobytes => $"{res / MemoryTierSize} KB",
                MemoryType.Bytes => $"{res} B",
                _ => "null"
            };
        }

        public static Task<DiscordMessage> SendFileToAsync(this MemoryStream stream,
            DiscordChannel channel, string text = null, bool isTts = false, DiscordEmbed embed = null,
            IEnumerable<IMention> allowedMentions = null)
        {
            return channel.SendFileAsync("", stream, text, isTts, embed, allowedMentions);
        }

        public static bool IsMatch(this Regex regex, string str, out Match match)
        {
            match = regex.Match(str);
            return match.Success;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();

        public static string AsPrettyString(this Type type)
        {
            string FormatTypeName(Type t)
                => t.Name switch
                {
                    "Boolean" => "bool",
                    "Byte" => "byte",
                    "SByte" => "sbyte",
                    "Int16" => "short",
                    "UInt16" => "ushort",
                    "Int32" => "int",
                    "UInt32" => "uint",
                    "Int64" => "long",
                    "UInt64" => "ulong",
                    "Char" => "char",
                    "String" => "string",
                    _ => t.Name
                };

            var types = type.GenericTypeArguments;

            //thanks .NET for putting an annoying ass backtick and number at the end of type names.
            var vs = FormatTypeName(type).Replace($"`{types.Length}", "");

            if (!types.IsEmpty()) vs += $"<{types.Select(FormatTypeName).Join(", ")}>";

            return vs;
        }

        public static IEnumerable<T> GetFlags<T>(this T input) where T : Enum
        {
            return Enumerable.Cast<T>(Enum.GetValues(input.GetType())).Where(e => input.HasFlag(e));
        }

        public static StringBuilder AppendAllLines(this StringBuilder sb, params string[] lines)
        {
            foreach (var line in lines)
            {
                sb.AppendLine(line);
            }

            return sb;
        }
    }

    public enum MemoryType
    {
        Terabytes,
        Gigabytes,
        Megabytes,
        Kilobytes,
        Bytes
    }
}