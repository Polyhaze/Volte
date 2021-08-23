using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Qmmands;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule
    {
        [Command("CountMembers", "Cm")]
        [Description("Counts the amount of members in the given role.")]
        public async Task<ActionResult> CountMembersAsync(
            [Remainder, Description("The role in which you want to count members for.")]
            SocketRole role)
        {
            var users = (await Context.Guild.GetUsersAsync().FlattenAsync())
                .Where(x => x.RoleIds.Contains(role.Id))
                .ToArray();

            return Ok(sb =>
            {
                sb.Append($"There {"is".ToQuantity(users.Length).Split(" ")[1]} {"member".ToQuantity(users.Length)} in the role {role.Mention}");
                sb.Append(users.Any(x => x.Id == Context.User.Id)
                    ? "; including you."
                    : ".");
            });
        }
    }
}