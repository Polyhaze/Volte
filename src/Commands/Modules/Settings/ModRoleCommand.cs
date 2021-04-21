using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;

namespace Volte.Commands.Modules
{
    public sealed partial class SettingsModule
    {
        [Command("ModRole", "Mod")]
        [Description("Sets the role able to use Moderation commands for the current guild.")]
        public Task<ActionResult> ModRoleAsync(
            [Remainder,
             Description("The role to be set as the Moderator role; or none if you want to see the current one.")]
            SocketRole role = null)
        {
            if (role is null)
                return Ok(
                    $"The current Moderator role in this guild is <@&{Context.GuildData.Configuration.Moderation.ModRole}>.");

            Context.Modify(data => data.Configuration.Moderation.ModRole = role.Id);
            return Ok($"Set {role.Mention} as the Moderator role for this guild.");
        }
    }
}