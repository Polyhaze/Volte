using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;
using Gommon;

namespace Volte.Commands.Modules
{
    public sealed partial class ModerationModule
    {
        [Command("Bans")]
        [Description("Shows all bans in this guild.")]
        [Remarks("bans")]
        [RequireBotGuildPermission(Permissions.BanMembers)]
        public async Task<ActionResult> BansAsync()
        {
            var banList = await Context.Guild.GetBansAsync();
            if (banList.IsEmpty()) return BadRequest("This guild doesn't have anyone banned.");
            else
            {
                return None(async () =>
                {
                    await Context.Interactivity.SendPaginatedMessageAsync(Context.Channel, Context.Member,
                        banList.Select(x => $"**{x.User}**: `{x.Reason ?? "No reason provided."}`").GetPages(10));
                }, false);
            }
        }
    }
}