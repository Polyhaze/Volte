using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Models.Results;
using Volte.Extensions;

namespace Volte.Commands.Modules
{
    public partial class AdminModule : VolteModule
    {
        [Command("Autorole")]
        [Description("Sets the role to be used for Autorole.")]
        [Remarks("Usage: |prefix|autorole {roleName}")]
        [RequireGuildAdmin]
        public Task<VolteCommandResult> AutoroleAsync([Remainder] SocketRole role)
        {
            var data = Db.GetData(Context.Guild);
            data.Configuration.Autorole = role.Id;
            Db.UpdateData(data);
            return Ok($"Successfully set **{role.Name}** as the role to be given to members upon joining this server.");
        }
    }
}