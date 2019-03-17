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
        [Command("Kick")]
        [Description("Kicks the given user.")]
        [Remarks("Usage: $kick {@user} [reason]")]
        [RequireBotGuildPermission(Permissions.KickMembers)]
        [RequireGuildModerator]
        public async Task KickAsync(DiscordMember user, [Remainder] string reason = "Kicked by a Moderator.")
        {
            try
            {
                await Context.CreateEmbed($"You were kicked from **{Context.Guild.Name}** for **{reason}**.")
                    .SendToAsync(user);
            }
            catch (UnauthorizedException) { }


            await user.RemoveAsync(reason);
            await Context.CreateEmbed($"Successfully kicked **{user.Username}#{user.Discriminator}** from this server.")
                .SendToAsync(Context.Channel);
        }
    }
}