using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Color", "Colour")]
        [Description("Shows the Hex and RGB representation for a given role in the current server.")]
        [Remarks("Usage: |prefix|color {role}")]
        public async Task RoleColorAsync([Remainder] SocketRole role)
        {
            await Context.CreateEmbedBuilder(
                $"**Dec:** {role.Color.RawValue}" +
                "\n" +
                $"**RGB:** {role.Color.R}, {role.Color.G}, {role.Color.B}"
            ).SendToAsync(Context.Channel);
        }
    }
}