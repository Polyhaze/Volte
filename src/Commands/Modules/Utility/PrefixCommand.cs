using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Prefix")]
        [Description("Shows the command prefix for this guild.")]
        [Remarks("Usage: |prefix|prefix")]
        public Task<VolteCommandResult> PrefixAsync()
        {
            return Ok($"The prefix for this server is **{Db.GetData(Context.Guild).Configuration.CommandPrefix}**.");
        }
    }
}