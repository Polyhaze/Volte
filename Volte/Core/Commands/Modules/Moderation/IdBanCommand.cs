using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Moderation {
    public partial class ModerationModule : VolteModule {
        [Command("IdBan")]
        [Description("Bans a user based on their ID.")]
        [Remarks("Usage: $idban {id} [reason]")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireGuildModerator]
        public async Task IdBan(ulong user, [Remainder] string reason = "Banned by a Moderator.") {
            await Context.Guild.AddBanAsync(user, 0, reason);
            await Context.CreateEmbed("Successfully banned that user from this guild.").SendTo(Context.Channel);
        }
    }
}