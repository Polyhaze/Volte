using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("RoleColor", "RoleColour")]
        [Description("Shows the Hex and RGB representation for a given role in the current server.")]
        [Remarks("Usage: |prefix|rolecolor {role}")]
        public async Task RoleColorAsync(SocketRole role)
        {
            await Context.CreateEmbedBuilder(
                    $"**Hex:** {role.Color.ToString().ToUpper()}" +
                    "\n" +
                    $"**RGB:** {role.Color.R}, {role.Color.G}, {role.Color.B}"
                ).SendTo(Context.Channel);
        }
    }
}