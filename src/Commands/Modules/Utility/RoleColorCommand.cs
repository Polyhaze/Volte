using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("RoleColor", "RoleColour")]
        [Description("Shows the Hex and RGB representation for a given role in the current server.")]
        [Remarks("Usage: |prefix|rolecolor {role}")]
        public async Task RoleColorAsync(DiscordRole role)
        {
            await Context.CreateEmbedBuilder(
                    $"**Dec:** {role.Color.Value}" +
                    "\n" +
                    $"**RGB:** {role.Color.R}, {role.Color.G}, {role.Color.B}"
                ).SendToAsync(Context.Channel);
        }
    }
}