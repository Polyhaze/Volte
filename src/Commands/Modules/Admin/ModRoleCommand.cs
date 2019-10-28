using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("ModRole")]
        [Description("Sets the role able to use Moderation commands for the current guild.")]
        [Remarks("modrole {role}")]
        [RequireGuildAdmin]
        public Task<ActionResult> ModRoleAsync([Remainder] SocketRole role)
        {
            Context.GuildData.Configuration.Moderation.ModRole = role.Id;
            Db.UpdateData(Context.GuildData);
            return Ok($"Set **{role.Name}** as the Moderator role for this guild.");
        }
    }
}