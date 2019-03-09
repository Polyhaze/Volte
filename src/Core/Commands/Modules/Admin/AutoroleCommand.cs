using System.Linq;
using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("Autorole")]
        [Description("Sets the role to be used for Autorole.")]
        [Remarks("Usage: |prefix|autorole {roleName}")]
        [RequireGuildAdmin]
        public async Task AutoroleAsync([Remainder] string roleName)
        {
            var config = Db.GetConfig(Context.Guild);
            var role = Context.Guild.Roles
                .FirstOrDefault(r => r.Name.EqualsIgnoreCase(roleName));
            if (role is null)
            {
                await Context.CreateEmbed($"The specified role, **{roleName}**, doesn't exist in this guild.")
                    .SendToAsync(Context.Channel);
                return;
            }

            config.Autorole = role.Name;
            Db.UpdateConfig(config);
            await Context.CreateEmbed($"Successfully set **{role.Name}** as the role to be given to members upon joining this server.")
                .SendToAsync(Context.Channel);
        }
    }
}