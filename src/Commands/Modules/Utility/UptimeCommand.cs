using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;
using Volte.Helpers;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Uptime")]
        [Description("Shows the bot's uptime in a human-friendly fashion.")]
        [Remarks("Usage: |prefix|uptime")]
        public Task<ActionResult> UptimeAsync() 
            => Ok($"I've been online for **{CalculationHelper.GetUptime()}**!");
    }
}