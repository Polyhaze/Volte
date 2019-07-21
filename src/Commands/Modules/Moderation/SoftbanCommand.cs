using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Models;
using Volte.Data.Models.EventArgs;
using Volte.Data.Models.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public partial class ModerationModule : VolteModule
    {
        [Command("Softban")]
        [Description("Softbans the mentioned user, kicking them and deleting the last 7 days of messages.")]
        [Remarks("Usage: |prefix|softban {@user} {daysToDelete} [reason]")]
        [RequireBotGuildPermission(GuildPermission.KickMembers | GuildPermission.BanMembers)]
        [RequireGuildModerator]
        public async Task<VolteCommandResult> SoftBanAsync(SocketGuildUser user, int daysToDelete,
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

            return Ok($"Successfully softbanned **{user.Username}#{user.Discriminator}**.", _ =>
                ModLogService.OnModActionCompleteAsync(new ModActionEventArgs(Context, ModActionType.Softban, user,
                    reason)));
        }
    }
}