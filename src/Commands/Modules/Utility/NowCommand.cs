using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Now")]
        [Description("Shows the current date and time, in UTC.")]
        [Remarks("now")]
        public Task<ActionResult> NowAsync()
            => Ok(new DiscordEmbedBuilder().WithDescription(new StringBuilder()
                .AppendLine($"**Date**: {Context.Now.FormatDate()} UTC")
                .AppendLine($"**Time**: {Context.Now.FormatFullTime()} UTC")
                .ToString()).WithCurrentTimestamp());
    }
}