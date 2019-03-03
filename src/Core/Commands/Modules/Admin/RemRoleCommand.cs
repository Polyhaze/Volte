using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("RemRole", "Rr")]
        [Description("Remove a role from the mentioned user.")]
        [Remarks("Usage: |prefix|remrole {@user} {roleName}")]
        [RequireGuildAdmin]
        public async Task RemRoleAsync(SocketGuildUser user, [Remainder] string roleName)
        {
            var targetRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(roleName));
            if (targetRole is null)
            {
                await Context.CreateEmbed($"**{roleName}** doesn't exist on this server!").SendTo(Context.Channel);
                
            }
            else
            {
                await user.RemoveRoleAsync(targetRole);
                await Context.CreateEmbed($"Removed the role **{roleName}** from {user.Mention}!").SendTo(Context.Channel);
            }

            
        }
    }
}