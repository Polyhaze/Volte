using System.Linq;
using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("ModRole")]
        [Description("Sets the role able to use Moderation commands for the current guild.")]
        [Remarks("Usage: |prefix|modrole {roleName}")]
        [RequireGuildAdmin]
        public async Task ModRoleAsync([Remainder] string roleName)
        {
            var config = Db.GetConfig(Context.Guild);
            var role = Context.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(roleName));
            if (role is null)
            {
                await Context.CreateEmbed($"{roleName} doesn't exist in this server.").SendTo(Context.Channel);
            }
            else
            {
                config.ModRole = role.Id;
                Db.UpdateConfig(config);
                await Context.CreateEmbed($"Set **{role.Name}** as the Moderator role for this server.")
                    .SendTo(Context.Channel);
            }
        }
    }
}