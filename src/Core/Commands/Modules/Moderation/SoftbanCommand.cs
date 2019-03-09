using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Moderation
{
    public partial class ModerationModule : VolteModule
    {
        [Command("Softban")]
        [Description("Softbans the mentioned user, kicking them and deleting the last 7 days of messages.")]
        [Remarks("Usage: $softban {@user} {daysToDelete} [reason]")]
        [RequireBotGuildPermission(GuildPermission.KickMembers | GuildPermission.BanMembers)]
        [RequireGuildModerator]
        public async Task SoftBanAsync(SocketGuildUser user, int daysToDelete,
            [Remainder] string reason = "Softbanned by a Moderator.")
        {
            try
            {
                await Context.CreateEmbed($"You've been softbanned from **{Context.Guild.Name}** for **{reason}**.")
                    .SendToAsync(user);
            }
            catch (HttpException ignored) when (ignored.DiscordCode == 50007) { }

            await user.BanAsync(daysToDelete, reason);
            await Context.Guild.RemoveBanAsync(user);
            await Context.CreateEmbed($"Successfully softbanned **{user.Username}#{user.Discriminator}**.")
                .SendToAsync(Context.Channel);
        }
    }
}