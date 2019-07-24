using System;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Now")]
        [Description("Shows the current date and time, in UTC.")]
        [Remarks("Usage: |prefix|now")]
        public Task<ActionResult> NowAsync()
        {
            return Ok($"**Date**: {DateTimeOffset.UtcNow.FormatDate()} UTC\n" +
                      $"**Time**: {DateTimeOffset.UtcNow.FormatFullTime()} UTC");
        }
    }
}