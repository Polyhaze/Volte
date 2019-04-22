using System.Linq;
using System.Threading.Tasks;
using Qmmands;
using Volte.Extensions;
using Gommon;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Iam")]
        [Description("Gives yourself a role, if it is in the current guild's self role list.")]
        [Remarks("Usage: |prefix|iam {roleName}")]
        public async Task IamAsync([Remainder] string roleName)
        {
            var config = Db.GetConfig(Context.Guild);
            if (!config.SelfRoles.Any(x => x.EqualsIgnoreCase(roleName)))
            {
                await Context.CreateEmbed($"The role **{roleName}** isn't in the self roles list for this guild.")
                    .SendToAsync(Context.Channel);
            }
            else
            {
                var target = Context.Guild.Roles.FirstOrDefault(x => x.Name.EqualsIgnoreCase(roleName));
                if (target is null)
                {
                    await Context.CreateEmbed($"The role **{roleName}** doesn't exist in this guild.")
                        .SendToAsync(Context.Channel);
                }
                else
                {
                    await Context.User.AddRoleAsync(target);
                    await Context.CreateEmbed($"Gave you the **{roleName}** role.").SendToAsync(Context.Channel);
                }
            }
        }
    }
}