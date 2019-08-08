using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Gommon;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule : VolteModule
    {
        [Command("Bans")]
        [Description("Shows all bans in this server.")]
        [Remarks("Usage: |prefix|bans")]
        [RequireBotGuildPermission(GuildPermission.BanMembers)]
        [RequireGuildModerator]
        public async Task<ActionResult> BansAsync()
        {
            var banList = await Context.Guild.GetBansAsync();
            return !banList.Any()
                ? BadRequest("This server doesn't have anyone banned.")
                : Ok(banList.Select(b => $"**{b.User}**: `{b.Reason ?? "No reason provided."}`").Join('\n'));
        }
    }
}