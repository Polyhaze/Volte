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
        [Command("Now")]
        [Description("Shows the current date and time.")]
        public Task<ActionResult> NowAsync()
            => Ok(new EmbedBuilder().WithDescription(new StringBuilder()
                .AppendLine($"**Date**: {Context.Now.FormatDate()}")
                .AppendLine($"**Time**: {Context.Now.FormatFullTime()}")
                .ToString()).WithCurrentTimestamp());
    }
}