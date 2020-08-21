using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule
    {
        [Command("AdminRole")]
        [Description("Sets the role able to use Admin commands for the current guild.")]
        [Remarks("adminrole {Role}")]
        public Task<ActionResult> AdminRoleAsync([Remainder]DiscordRole role)
        {
            ModifyData(data =>
            {
                data.Configuration.Moderation.AdminRole = role.Id;
                return data;
            });
            return Ok($"Set **{role.Name}** as the Admin role for this guild.");
        }
    }
}