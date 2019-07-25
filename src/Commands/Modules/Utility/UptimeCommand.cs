using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Humanizer;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Uptime")]
        [Description("Shows the bot's uptime in a human-friendly fashion.")]
        [Remarks("Usage: |prefix|uptime")]
        public Task<ActionResult> UptimeAsync() 
            => Ok($"I've been online for **{(DateTime.Now - Process.GetCurrentProcess().StartTime).Humanize(3)}**!");
    }
}