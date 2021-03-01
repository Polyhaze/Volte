using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule
    {
        [Command("CountMembers", "Cm")]
        [Description("Counts the amount of members in the given role.")]
        [Remarks("countmembers {Role}")]
        public async Task<ActionResult> CountMembersAsync([Remainder] SocketRole role)
        {
            var users = await Context.Guild.GetUsersAsync().FlattenAsync();
            var usersInRole = users.Where(x => x.RoleIds.Contains(role.Id)).ToArray();
            var result = $"There are {usersInRole.Length} members in the role {role.Mention}";

            if (usersInRole.Any(x => x.Id == Context.User.Id))
            {
                return Ok($"{result}; including you.");
            }

            return Ok($"{result}.");
        }
    }
}