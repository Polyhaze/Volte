using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Snowflake")]
        [Description("Shows when the object with the given Snowflake ID was created.")]
        [Remarks("Usage: |prefix|snowflake {id}")]
        public async Task SnowflakeAsync(ulong id)
        {
            var date = SnowflakeUtils.FromSnowflake(id);
            await Context.CreateEmbedBuilder(
                    $"**Date:** {date.FormatDate()}\n" +
                    $"**Time**: {date.FormatFullTime()}"
                )
                .SendToAsync(Context.Channel);
        }
    }
}