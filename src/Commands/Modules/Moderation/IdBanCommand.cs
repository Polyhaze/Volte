using System.Threading.Tasks;
using DSharpPlus;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.Moderation
{
    public partial class ModerationModule : VolteModule
    {
        [Command("IdBan")]
        [Description("Bans a user based on their ID.")]
        [Remarks("Usage: $idban {id} [reason]")]
        [RequireBotGuildPermission(Permissions.BanMembers)]
        [RequireGuildModerator]
        public async Task IdBanAsync(ulong user, [Remainder] string reason = "Banned by a Moderator.")
        {
            await Context.Guild.BanMemberAsync(user, 0, reason);
            await Context.CreateEmbed("Successfully banned that user from this guild.").SendToAsync(Context.Channel);
        }
    }
}