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
        [Command("Kick")]
        [Description("Kicks the given user.")]
        [Remarks("Usage: |prefix|kick {@user} [reason]")]
        [RequireBotGuildPermission(GuildPermission.KickMembers)]
        [RequireGuildModerator]
        public async Task<VolteCommandResult> KickAsync(SocketGuildUser user,
            [Remainder] string reason = "Kicked by a Moderator.")
        {
            try
            {
                await Context.CreateEmbed($"You were kicked from **{Context.Guild.Name}** for **{reason}**.")
                    .SendToAsync(user);
            }
            catch (HttpException ignored) when (ignored.DiscordCode == 50007) { }

            await user.KickAsync(reason);

            return Ok($"Successfully kicked **{user.Username}#{user.Discriminator}** from this server.", _ =>
                ModLogService.OnModActionCompleteAsync(
                    new ModActionEventArgs(Context, ModActionType.Kick, user, reason)));
        }
    }
}