using System;
using Discord;
using Discord.Commands;

namespace Gommon
{
    public static partial class Extensions
    {
        //an extension on all `Type` instances, it's only applicable on typeparsers.
        public static string SanitizeParserName(this Type type)
        {
            return type.Name.Replace("Parser", string.Empty);
        }
    }
}