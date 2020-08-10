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
        [RequireGuildModerator]
        public async Task<ActionResult> BansAsync()
        {
            var banList = await Context.Guild.GetBansAsync();
            if (banList.IsEmpty()) return BadRequest("This guild doesn't have anyone banned.");
            else
            {
                var list = banList.Select(x => $"**{x.User}**: `{x.Reason ?? "No reason provided."}`").ToList();
                var pages = new List<string>();

                do
                {
                    pages.Add(list.Take(10).Join("\n"));
                    list.RemoveRange(0, list.Count < 10 ? list.Count : 10);
                } while (!list.IsEmpty());
                
                return Ok(async () =>
                {
                    await PagedReplyAsync(new PaginatedMessage
                    {
                        Author = Context.User,
                        Pages = pages
                    });
                }, false);
            }
        }
    }
}