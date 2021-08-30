using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Entities;
using Gommon;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule
    {
        [Command("Bans")]
        [Description("Shows all bans in this guild.")]
        public async Task<ActionResult> BansAsync()
        {
            var banList = await Context.Guild.GetBansAsync();
            return !banList.Any()
                ? BadRequest("This guild doesn't have anyone banned.")
                : Ok(banList.Select(b => $"**{b.User}**: {Format.Code(b.Reason ?? "No reason provided.")}").Join('\n'));
        }
    }
}