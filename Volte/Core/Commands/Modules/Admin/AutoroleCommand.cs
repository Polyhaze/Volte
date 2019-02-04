using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;
using Volte.Core.Data;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Admin {
    public partial class AdminModule : VolteModule {
        [Command("Autorole")]
        [Summary("Sets the role to be used for Autorole.")]
        [Remarks("Usage: |prefix|autorole {roleName}")]
        [RequireGuildAdmin]
        public async Task Autorole([Remainder]string role) {
            var config = Db.GetConfig(Context.Guild);
            var roleToApply = Context.Guild.Roles
                .FirstOrDefault(r => r.Name.EqualsIgnoreCase(role));
            if (roleToApply is null) {
                await Reply(Context.Channel,
                    CreateEmbed(Context, $"The specified role, **{role}**, doesn't exist on this guild."));
                return;
            }

            config.Autorole = roleToApply.Name;
            Db.UpdateConfig(config);
            await Reply(Context.Channel, CreateEmbed(Context,
                $"Successfully set **{roleToApply.Name}** as the Autorole for this server."));
        }
    }
}