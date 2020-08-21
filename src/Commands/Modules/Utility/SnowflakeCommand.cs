using System;
using System.Text;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Snowflake")]
        [Description("Shows when the object with the given Snowflake ID was created, in UTC.")]
        [Remarks("snowflake {Ulong}")]
        public Task<ActionResult> SnowflakeAsync(ulong id)
        {
            var date = SnowflakeUtils.FromSnowflake(id);
            return Ok(new StringBuilder()
                .AppendLine($"**Date:** {date.FormatDate()}")
                .AppendLine($"**Time**: {date.FormatFullTime()}")
                .ToString());
        }

        /// <summary>
        ///     Provides a series of helper methods for handling snowflake identifiers.
        /// </summary>
        /// <remarks>Taken from <a href="https://github.com/discord-net/Discord.Net/blob/ff0fea98a65d907fbce07856f1a9ef4aebb9108b/src/Discord.Net.Core/Utils/SnowflakeUtils.cs">Discord.Net</a> source.</remarks>
        private static class SnowflakeUtils
        {
            /// <summary>
            ///     Resolves the time of which the snowflake is generated.
            /// </summary>
            /// <param name="value">The snowflake identifier to resolve.</param>
            /// <returns>
            ///     A <see cref="DateTimeOffset" /> representing the time for when the object is geenrated.
            /// </returns>
            public static DateTimeOffset FromSnowflake(ulong value)
                => DateTimeOffset.FromUnixTimeMilliseconds((long)((value >> 22) + 1420070400000UL));
            /// <summary>
            ///     Generates a pseudo-snowflake identifier with a <see cref="DateTimeOffset"/>.
            /// </summary>
            /// <param name="value">The time to be used in the new snowflake.</param>
            /// <returns>
            ///     A <see cref="UInt64" /> representing the newly generated snowflake identifier.
            /// </returns>
            public static ulong ToSnowflake(DateTimeOffset value)
                => ((ulong)value.ToUnixTimeMilliseconds() - 1420070400000UL) << 22;
        }
    }
}