using System;
using System.Text;
using System.Threading.Tasks;
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
            => Ok(new StringBuilder()
                .AppendLine($"**Date**: {Context.Now.FormatDate()} UTC")
                .AppendLine($"**Time**: {Context.Now.FormatFullTime()} UTC")
                .ToString());
    }
}