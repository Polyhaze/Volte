using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Objects;
using Volte.Data.Objects.EventArgs;
using Volte.Extensions;
using Gommon;

namespace Volte.Commands.Modules.Moderation
{
    public partial class ModerationModule : VolteModule
    {
        [Command("Purge", "clear", "clean")]
        [Description("Purges the last x messages.")]
        [Remarks("Usage: $purge {count}")]
        [RequireBotChannelPermission(ChannelPermission.ManageMessages)]
        [RequireGuildModerator]
        public async Task PurgeAsync(int count)
        {
            await Context.Channel.DeleteMessagesAsync(await Context.Channel.GetMessagesAsync(count + 1).FlattenAsync());
            //+1 to include the command invocation message, and actually delete the last x messages instead of x - 1.
            //lets you theoretically use 0 to delete only the invocation message, for testing or something.
            var msg = await Context
                .CreateEmbed($"Successfully deleted **{count}** message{(count.ShouldBePlural() ? "s" : string.Empty)}")
                .SendToAsync(Context.Channel);
            await Executor.ExecuteAfterDelayAsync(3000, async () => await msg.DeleteAsync());
            await EventService.OnModActionCompleteAsync(new ModActionEventArgs(Context, ModActionType.Purge, count));
        }
    }
}