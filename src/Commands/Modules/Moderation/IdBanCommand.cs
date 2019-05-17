using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Models;
using Volte.Data.Models.EventArgs;
using Volte.Extensions;

namespace Volte.Commands.Modules.Moderation
{
    public partial class ModerationModule : VolteModule
    {
        [Command("IdBan")]
        [Description("Bans a user based on their ID.")]
        [Remarks("Usage: |prefix|idban {id} [reason]")]
        [RequireBotGuildPermission(GuildPermission.BanMembers)]
        [RequireGuildModerator]
        public async Task IdBanAsync(ulong user, [Remainder] string reason = "Banned by a Moderator.")
        {
            await Context.Guild.AddBanAsync(user, 0, reason);
            await Context.CreateEmbed("Successfully banned that user from this guild.").SendToAsync(Context.Channel);
            await ModLogService.OnModActionCompleteAsync(new ModActionEventArgs(Context, ModActionType.IdBan, user,
                reason));
        }
    }
}