using System.Text;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Commands;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Snowflake", "Id")]
        [Description("Shows when the object with the given Snowflake ID was created, in UTC.")]
        public Task<ActionResult> SnowflakeAsync([Description("The Discord snowflake you want to see.")] ulong id) =>
            Ok(Context.CreateEmbedBuilder().WithTitle(SnowflakeUtils.FromSnowflake(id).GetDiscordTimestamp(TimestampType.LongDateTime)));
    }
}