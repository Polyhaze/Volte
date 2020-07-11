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
        [Remarks("now")]
        public Task<ActionResult> NowAsync()
            => Ok(new EmbedBuilder().WithDescription(new StringBuilder()
                .AppendLine($"**Date**: {Context.Now.FormatDate()} UTC")
                .AppendLine($"**Time**: {Context.Now.FormatFullTime()} UTC")
                .ToString()).WithCurrentTimestamp());
    }
}