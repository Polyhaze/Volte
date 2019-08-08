using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
 
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Now")]
        [Description("Shows the current date and time, in UTC.")]
        [Remarks("Usage: |prefix|now")]
        public Task<ActionResult> NowAsync()
        {
            return Ok(new StringBuilder()
                .AppendLine($"**Date**: {DateTimeOffset.UtcNow.FormatDate()} UTC")
                .AppendLine($"**Time**: {DateTimeOffset.UtcNow.FormatFullTime()} UTC")
                .ToString());
        }
    }
}