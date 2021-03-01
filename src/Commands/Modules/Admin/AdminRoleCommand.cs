using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Attributes;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule
    {
        [Command("AdminRole")]
        [Description("Sets the role able to use Admin commands for the current guild.")]
        [Remarks("adminrole [Role]")]
        [RequireGuildAdmin]
        public Task<ActionResult> AdminRoleAsync([Remainder] SocketRole role = null)
        {
            if (role is null)
            {
                return Ok($"The current Admin role in this guild is <@{Context.GuildData.Configuration.Moderation.AdminRole}>.");
            }
            Context.GuildData.Configuration.Moderation.AdminRole = role.Id;
            Db.UpdateData(Context.GuildData);
            return Ok($"Set {role.Mention} as the Admin role for this guild.");
        }
    }
}