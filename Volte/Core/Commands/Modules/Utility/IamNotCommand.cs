using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility {
    public partial class UtilityModule : VolteModule {
        [Command("IamNot")]
        [Summary("Take a role from yourself, if it is in the current guild's self role list.")]
        [Remarks("Usage: |prefix|iamnot {roleName}")]
        public async Task IamNot([Remainder] string roleName) {
            var config = Db.GetConfig(Context.Guild);
            if (!config.SelfRoles.Any(x => x.EqualsIgnoreCase(roleName))) {
                await Context.CreateEmbed($"The role **{roleName}** isn't in the self roles list for this guild.")
                    .SendTo(Context.Channel);
            }
            else {
                var target = Context.Guild.Roles.FirstOrDefault(x => x.Name.EqualsIgnoreCase(roleName));
                if (target is null) {
                    await Context.CreateEmbed($"The role **{roleName}** doesn't exist in this guild")
                        .SendTo(Context.Channel);
                }
                else {
                    await Context.GuildUser.RemoveRoleAsync(target);
                    await Context.CreateEmbed($"Took away your **{roleName}** role.").SendTo(Context.Channel);
                }
            }
        }
    }
}