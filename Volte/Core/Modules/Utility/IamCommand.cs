using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Volte.Core.Extensions;

namespace Volte.Core.Modules.Utility {
    public partial class UtilityModule : VolteModule {
        [Command("Iam")]
        [Summary("Gives yourself a role, if it is in the current guild's self role list.")]
        [Remarks("Usage: |prefix|iam {roleName}")]
        public async Task Iam([Remainder] string roleName) {
            var config = Db.GetConfig(Context.Guild);
            if (!config.SelfRoles.Any(x => x.EqualsIgnoreCase(roleName))) {
                await Reply(Context.Channel, CreateEmbed(Context, 
                    $"The role **{roleName}** isn't in the self roles list for this guild."));
            }
            else {
                var target = Context.Guild.Roles.FirstOrDefault(x => x.Name.EqualsIgnoreCase(roleName));
                if (target is null) {
                    await Reply(Context.Channel,
                        CreateEmbed(Context, $"The role **{roleName}** doesn't exist in this guild"));
                }
                else {
                    await Context.GuildUser.AddRoleAsync(target);
                    await Reply(Context.Channel, CreateEmbed(Context, $"Gave you the **{roleName}** role."));
                }
            }
        }
    }
}