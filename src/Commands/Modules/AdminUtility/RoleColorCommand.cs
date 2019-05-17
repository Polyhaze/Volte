using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.AdminUtility
{
    public partial class AdminUtilityModule : VolteModule
    {
        [Command("RoleColor", "RoleClr", "Rcl")]
        [Description("Changes the color of a specified role.")]
        [Remarks("Usage: |prefix|rolecolor {role} {r} {g} {b}")]
        [RequireGuildAdmin]
        public async Task RoleColorAsync(SocketRole role, int r, int g, int b)
        {
            await role.ModifyAsync(x => x.Color = new Color(r, g, b));
            await Context.CreateEmbed($"Successfully changed the color of the role **{role.Name}**.")
                .SendToAsync(Context.Channel);
        }
    }
}