using System.Text;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Commands;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Snowflake", "Id")]
        [Description("Shows when the object with the given Snowflake ID was created, in UTC.")]
        public Task<ActionResult> SnowflakeAsync([Description("The Discord snowflake you want to see.")]
            ulong id)
        {
            var date = SnowflakeUtils.FromSnowflake(id);
            return Ok(new StringBuilder()
                .AppendLine($"**Date:** {date.FormatDate()}")
                .AppendLine($"**Time**: {date.FormatFullTime()}"));
        }
    }
}