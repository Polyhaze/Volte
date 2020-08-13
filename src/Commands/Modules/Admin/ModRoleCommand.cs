using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule
    {
        [Command("ModRole")]
        [Description("Sets the role able to use Moderation commands for the current guild.")]
        [Remarks("modrole {Role}")]
        public Task<ActionResult> ModRoleAsync([Remainder] SocketRole role)
        {
            ModifyData(data =>
            {
                data.Configuration.Moderation.ModRole = role.Id;
                return data;
            });
            return Ok($"Set **{role.Name}** as the Moderator role for this guild.");
        }
    }
}