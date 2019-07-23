using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Core.Data.Models.Results;

namespace Volte.Commands.Modules
{
    public partial class AdminModule : VolteModule
    {
        [Command("ModRole")]
        [Description("Sets the role able to use Moderation commands for the current guild.")]
        [Remarks("Usage: |prefix|modrole {role}")]
        [RequireGuildAdmin]
        public Task<VolteCommandResult> ModRoleAsync(SocketRole role)
        {
            var data = Db.GetData(Context.Guild);
            data.Configuration.Moderation.ModRole = role.Id;
            Db.UpdateData(data);
            return Ok($"Set **{role.Name}** as the Moderator role for this server.");
        }
    }
}