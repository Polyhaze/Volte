using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("Autorole")]
        [Description("Sets the role to be used for Autorole.")]
        [Remarks("Usage: |prefix|autorole {roleName}")]
        [RequireGuildAdmin]
        public async Task AutoroleAsync([Remainder] SocketRole role)
        {
            var config = Db.GetConfig(Context.Guild);
            if (role is null)
            {
                await Context.CreateEmbed($"The specified role, **{role.Name}**, doesn't exist in this guild.")
                    .SendToAsync(Context.Channel);
                return;
            }

            config.Autorole = role.Name;
            Db.UpdateConfig(config);
            await Context
                .CreateEmbed(
                    $"Successfully set **{role.Name}** as the role to be given to members upon joining this server.")
                .SendToAsync(Context.Channel);
        }
    }
}