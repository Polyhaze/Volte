using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("IamNot")]
        [Description("Take a role from yourself, if it is in the current guild's self role list.")]
        [Remarks("Usage: |prefix|iamnot {roleName}")]
        public async Task IamNotAsync([Remainder] string roleName)
        {
            var data = Db.GetData(Context.Guild);
            if (!data.Extras.SelfRoles.Any(x => x.EqualsIgnoreCase(roleName)))
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
                    await Context.User.RemoveRoleAsync(target);
                    await Context.CreateEmbed($"Took away your **{roleName}** role.").SendToAsync(Context.Channel);
                }
            }
        }
    }
}