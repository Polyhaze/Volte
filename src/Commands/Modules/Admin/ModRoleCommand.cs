using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule
    {
        [Command("ModRole")]
        [Description("Sets the role able to use Moderation commands for the current guild.")]
        [Remarks("modrole [Role]")]
        [RequireGuildAdmin]
        public Task<ActionResult> ModRoleAsync([Remainder] SocketRole role = null)
        {
            if (role is null)
            {
                return Ok($"The current Moderator role in this guild is <@{Context.GuildData.Configuration.Moderation.ModRole}>.");
            }
            Context.GuildData.Configuration.Moderation.ModRole = role.Id;
            Db.UpdateData(Context.GuildData);
            return Ok($"Set {role.Mention} as the Moderator role for this guild.");
        }
    }
}