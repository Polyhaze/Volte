using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Core.Data.Models.Results;

namespace Volte.Commands.Modules
{
    public partial class AdminModule : VolteModule
    {
        [Command("Antilink", "Al")]
        [Description("Enable/Disable Antilink for the current guild.")]
        [Remarks("Usage: |prefix|antilink {true|false}")]
        [RequireGuildAdmin]
        public Task<VolteCommandResult> AntilinkAsync(bool enabled)
        {
            var data = Db.GetData(Context.Guild);
            data.Configuration.Moderation.Antilink = enabled;
            Db.UpdateData(data);
            return Ok(enabled ? "Antilink has been enabled." : "Antilink has been disabled.");
        }
    }
}