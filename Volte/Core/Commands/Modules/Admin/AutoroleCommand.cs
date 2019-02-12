using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("Autorole")]
        [Summary("Sets the role to be used for Autorole.")]
        [Remarks("Usage: |prefix|autorole {roleName}")]
        [RequireGuildAdmin]
        public async Task Autorole([Remainder] string role) {
            var config = Db.GetConfig(Context.Guild);
            var roleToApply = Context.Guild.Roles
                .FirstOrDefault(r => r.Name.EqualsIgnoreCase(role));
            if (roleToApply is null) {
                await Context.CreateEmbed($"The specified role, **{role}**, doesn't exist on this guild.")
                    .SendTo(Context.Channel);
                return;
            }

            config.Autorole = roleToApply.Name;
            Db.UpdateConfig(config);
            await Context.CreateEmbed($"Successfully set **{roleToApply.Name}** as the Autorole for this server.")
                .SendTo(Context.Channel);
        }
    }
}