using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands;

namespace Volte.Commands.Modules
{
    public sealed partial class SettingsModule
    {
        [Command("AdminRole", "Admin")]
        [Description("Sets the role able to use Admin commands for the current guild.")]
        public Task<ActionResult> AdminRoleAsync([Remainder, Description("The role to be set as the Admin role; or none if you want to see the current one.")] SocketRole role = null)
        {
            if (role is null)
                return Ok($"The current Admin role in this guild is <@&{Context.GuildData.Configuration.Moderation.AdminRole}>.");
            
            Context.Modify(data => data.Configuration.Moderation.AdminRole = role.Id);
            return Ok($"Set {role.Mention} as the Admin role for this guild.");
        }
    }
}