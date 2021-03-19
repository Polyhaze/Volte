using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Entities;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule
    {
        [Command("Autorole")]
        [Description("Sets the role to be used for Autorole.")]
        [Remarks("autorole {Role}")]
        [RequireGuildAdmin]
        public Task<ActionResult> AutoroleAsync([Remainder] SocketRole role)
        {
            Context.GuildData.Configuration.Autorole = role.Id;
            Db.Save(Context.GuildData);
            return Ok($"Successfully set **{role.Name}** as the role to be given to members upon joining this guild.");
        }
    }
}