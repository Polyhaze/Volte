using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.Moderation
{
    public partial class ModerationModule : VolteModule
    {
        [Command("Softban")]
        [Description("Softbans the mentioned user, kicking them and deleting the last 0-7 days of messages.")]
        [Remarks("Usage: $softban {@user} {daysToDelete} [reason]")]
        [RequireBotGuildPermission(Permissions.KickMembers | Permissions.BanMembers)]
        [RequireGuildModerator]
        public async Task SoftBanAsync(DiscordMember user, int daysToDelete,
            [Remainder] string reason = "Softbanned by a Moderator.")
        {
            try
            {
                await Context.CreateEmbed($"You've been softbanned from **{Context.Guild.Name}** for **{reason}**.")
                    .SendToAsync(user);
            }
            catch (UnauthorizedException) { }

            await user.BanAsync(daysToDelete, reason);
            await Context.Guild.UnbanMemberAsync(user);
            await Context.CreateEmbed($"Successfully softbanned **{user.Username}#{user.Discriminator}**.")
                .SendToAsync(Context.Channel);
        }
    }
}