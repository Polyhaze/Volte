using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule : VolteModule
    {
        [Command("AdminRole")]
        [Description("Sets the role able to use Admin commands for the current guild.")]
        [Remarks("adminrole {role}")]
        [RequireGuildAdmin]
        public Task<ActionResult> AdminRoleAsync([Remainder] SocketRole role)
        {
            Context.GuildData.Configuration.Moderation.AdminRole = role.Id;
            Db.UpdateData(Context.GuildData);
            return Ok($"Set **{role.Name}** as the Admin role for this guild.");
        }
    }
}