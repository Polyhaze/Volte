using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Humanizer;
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
            var result = $"There {"is".ToQuantity(usersInRole.Length).Split(" ")[1]} {"member".ToQuantity(usersInRole.Length)} in the role {role.Mention}";

            return Ok(usersInRole.Any(x => x.Id == Context.User.Id) 
                ? $"{result}; including you." 
                : $"{result}.");
        }
    }
}