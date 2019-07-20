using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Data.Models.Results;
using Volte.Extensions;

namespace Volte.Commands.Modules
{
    public partial class AdminModule : VolteModule
    {
        [Command("ServerPrefix")]
        [Description("Sets the command prefix for this guild.")]
        [Remarks("Usage: |prefix|serverprefix {newPrefix}")]
        [RequireGuildAdmin]
        public Task<VolteCommandResult> ServerPrefixAsync([Remainder] string newPrefix)
        {
            var data = Db.GetData(Context.Guild);
            data.Configuration.CommandPrefix = newPrefix;
            Db.UpdateData(data);
            return Ok($"Set this server's prefix to **{newPrefix}**.");
        }
    }
}