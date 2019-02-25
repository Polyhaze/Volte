using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Moderation
{
    public partial class ModerationModule : VolteModule
    {
        [Command("Delete")]
        [Description("Deletes a message by its ID. Creates an audit log entry for abuse prevention.")]
        [Remarks("Usage: |prefix|delete {messageId}")]
        [RequireGuildModerator]
        [RequireBotChannelPermission(ChannelPermission.ManageMessages)]
        public async Task DeleteAsync(ulong messageId)
        {
            var target = await Context.Channel.GetMessageAsync(messageId);
            if (target is null)
            {
                await Context.CreateEmbed("That message doesn't exist in this channel.").SendTo(Context.Channel);
                return;
            }

            await target.DeleteAsync(new RequestOptions
            {
                AuditLogReason = $"Message deleted by Moderator {Context.User}."
            });

            var confirmationMessage = await Context
                .CreateEmbed($"{EmojiService.BALLOT_BOX_WITH_CHECK} Deleted that message.").SendTo(Context.Channel);
            await Task.Delay(2000).ContinueWith(async _ =>
            {
                await Context.Message.DeleteAsync();
                await confirmationMessage.DeleteAsync();
            });
        }
    }
}