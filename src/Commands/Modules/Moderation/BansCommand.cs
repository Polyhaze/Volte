using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;
using Gommon;
using Volte.Interactive;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule
    {
        [Command("Bans")]
        [Description("Shows all bans in this guild.")]
        [Remarks("bans")]
        [RequireBotGuildPermission(GuildPermission.BanMembers)]
        public async Task<ActionResult> BansAsync()
        {
            var banList = await Context.Guild.GetBansAsync();
            if (banList.IsEmpty()) return BadRequest("This guild doesn't have anyone banned.");
            else
            {
                return Ok(new PaginatedMessageBuilder(Context)
                    .WithPages(banList.Select(x => $"**{x.User}**: `{x.Reason ?? "No reason provided."}`"))
                    .SplitPages(10));
            }
        }
    }
}