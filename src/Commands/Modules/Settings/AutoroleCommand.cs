using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;

namespace Volte.Commands.Modules
{
    public sealed partial class SettingsModule
    {
        [Command("Autorole")]
        [Description("Sets the role to be used for Autorole.")]
        public Task<ActionResult> AutoroleAsync([Remainder, Description("The role to be given to users when they join; or none to see the current one.")] SocketRole role = null)
        {
            if (role is null)
                return Ok($"The current Autorole for this guild is <@&{Context.GuildData.Configuration.Autorole}>");

            Context.Modify(data => data.Configuration.Autorole = role.Id);
            return Ok($"Successfully set **{role.Name}** as the role to be given to members upon joining this guild.");
        }
    }
}