using System.Threading.Tasks;
using DSharpPlus;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;
using Volte.Utils;

namespace Volte.Commands.Modules.Moderation
{
    public partial class ModerationModule : VolteModule
    {
        [Command("Purge", "clear", "clean")]
        [Description("Purges the last x messages.")]
        [Remarks("Usage: $purge {count}")]
        [RequireBotChannelPermission(Permissions.ManageMessages)]
        [RequireGuildModerator]
        public async Task PurgeAsync(int count)
        {
            await Context.Channel.DeleteMessagesAsync(await Context.Channel.GetMessagesAsync(count + 1));
            //+1 to include the command invocation message, and actually delete the last x messages instead of x - 1.
            //lets you theoretically use 0 to delete only the invocation message, for testing or something.
            var msg = await Context
                .CreateEmbed($"Successfully deleted **{count}** {(count != 1 ? "messages" : "message")}.")
                .SendToAsync(Context.Channel);
            await ExecutorUtil.ExecuteAfterDelayAsync(3000, async () => await msg.DeleteAsync());
        }
    }
}