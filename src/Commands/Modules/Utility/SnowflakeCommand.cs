using System;
using System.Threading.Tasks;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Snowflake")]
        [Description("Shows when the object with the given Snowflake ID was created.")]
        [Remarks("Usage: |prefix|snowflake {id}")]
        public async Task SnowflakeAsync(ulong id)
        {
            var date = FromSnowflake(id);
            await Context.CreateEmbedBuilder(
                    $"**Date:** {date.FormatDate()}\n" +
                    $"**Time**: {date.FormatFullTime()}"
                )
                .SendToAsync(Context.Channel);
        }

        private DateTimeOffset FromSnowflake(ulong id) 
            => DateTimeOffset.FromUnixTimeMilliseconds((long)((id >> 22) + 1420070400000UL)); 
    }
}