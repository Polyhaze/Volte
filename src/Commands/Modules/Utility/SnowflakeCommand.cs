using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Data.Models.Results;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Snowflake")]
        [Description("Shows when the object with the given Snowflake ID was created, in UTC.")]
        [Remarks("Usage: |prefix|snowflake {id}")]
        public Task<VolteCommandResult> SnowflakeAsync(ulong id)
        {
            var date = SnowflakeUtils.FromSnowflake(id);
            return Ok($"**Date:** {date.FormatDate()}\n" +
                      $"**Time**: {date.FormatFullTime()}");
        }
    }
}