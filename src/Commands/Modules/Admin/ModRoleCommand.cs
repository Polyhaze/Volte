using System.Threading.Tasks;
using Discord;
 
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("ModRole")]
        [Description("Sets the role able to use Moderation commands for the current guild.")]
        [Remarks("Usage: |prefix|modrole {role}")]
        [RequireGuildAdmin]
        public Task<ActionResult> ModRoleAsync(SocketRole role)
        {
            Context.GuildData.Configuration.Moderation.ModRole = role.Id;
            Db.UpdateData(Context.GuildData);
            return Ok($"Set **{role.Name}** as the Moderator role for this server.");
        }
    }
}