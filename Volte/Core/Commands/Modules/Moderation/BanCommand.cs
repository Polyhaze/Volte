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
        [Command("Ban")]
        [Description("Bans the mentioned user.")]
        [Remarks("Usage: $ban {@user} {daysToDelete} [reason]")]
        [RequireBotGuildPermission(GuildPermission.BanMembers)]
        [RequireGuildModerator]
        public async Task BanAsync(SocketGuildUser user, int daysToDelete, [Remainder] string reason = "Banned by a Moderator.")
        {
            try
            {
                await Context.CreateEmbed($"You've been banned from **{Context.Guild.Name}** for **{reason}**.")
                    .SendTo(user);
            }
            catch (HttpException ignored) when (ignored.DiscordCode == 50007) { }

            await user.BanAsync(daysToDelete, reason);
            await Context.CreateEmbed($"Successfully banned **{user.Username}#{user.Discriminator}** from this guild.")
                .SendTo(Context.Channel);
        }
    }
}