using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule
    {
        [Command("Autorole")]
        [Description("Sets the role to be used for Autorole.")]
        [Remarks("autorole {Role}")]
        public Task<ActionResult> AutoroleAsync([Remainder]DiscordRole role)
        {
            ModifyData(data =>
            {
                data.Configuration.Autorole = role.Id;
                return data;
            });
            return Ok($"Successfully set **{role.Name}** as the role to be given to members upon joining this guild.");
        }
    }
}