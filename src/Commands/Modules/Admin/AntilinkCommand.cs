using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class AdminModule
    {
        [Command("Antilink", "Al")]
        [Description("Enable/Disable Antilink for the current guild.")]
        [Remarks("antilink {Boolean}")]
        public Task<ActionResult> AntilinkAsync(bool enabled)
        {
            ModifyData(data =>
            {
                data.Configuration.Moderation.Antilink = enabled;
                return data;
            });
            return Ok(enabled ? "Antilink has been enabled." : "Antilink has been disabled.");
        }
    }
}