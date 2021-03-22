using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands;
using Gommon;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule
    {
        [Command("Bans")]
        [Description("Shows all bans in this guild.")]
        [RequireBotGuildPermission(GuildPermission.BanMembers)]
        public async Task<ActionResult> BansAsync()
        {
            var banList = await Context.Guild.GetBansAsync();
            return !banList.Any()
                ? BadRequest("This guild doesn't have anyone banned.") 
                : Ok(banList.Select(b => $"**{b.User}**: `{b.Reason ?? "No reason provided."}`").Join('\n'));
        }
    }
}