using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.ServerAdmin
{
    public partial class ServerAdminModule : VolteModule
    {
        [Command("RoleColor", "RoleClr", "Rcl")]
        [Description("Changes the color of a specified role.")]
        [Remarks("Usage: |prefix|rolecolor {role} {r} {g} {b}")]
        [RequireGuildAdmin]
        public async Task RoleColorAsync(DiscordRole role, int r, int g, int b)
        {
            await role.ModifyAsync(x => x.Color = new DiscordColor(r, g, b));
            await Context.CreateEmbed($"Successfully changed the color of the role {role.Name}")
                .SendToAsync(Context.Channel);
        }
    }
}
