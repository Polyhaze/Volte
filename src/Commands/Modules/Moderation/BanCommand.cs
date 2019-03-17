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
        [Command("Ban")]
        [Description("Bans the mentioned user.")]
        [Remarks("Usage: $ban {@user} {daysToDelete} [reason]")]
        [RequireBotGuildPermission(Permissions.BanMembers)]
        [RequireGuildModerator]
        public async Task BanAsync(DiscordMember user, int daysToDelete, [Remainder] string reason = "Banned by a Moderator.")
        {
            try
            {
                await Context.CreateEmbed($"You've been banned from **{Context.Guild.Name}** for **{reason}**.")
                    .SendToAsync(user);
            }
            catch (UnauthorizedException) { }

            await user.BanAsync(daysToDelete, reason);
            await Context.CreateEmbed($"Successfully banned **{user.Username}#{user.Discriminator}** from this guild.")
                .SendToAsync(Context.Channel);
        }
    }
}