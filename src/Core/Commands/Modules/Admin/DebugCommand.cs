using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("Debug")]
        [Description("Generates a debug report for this guild.")]
        [Remarks("Usage: |prefix|debug")]
        [RequireGuildAdmin]
        public async Task DebugReportAsync()
        {
            await Context.CreateEmbed(
                    $"{DebugService.Execute(Db.GetConfig(Context.Guild).ToString())}" +
                    "\n\nTake this to the Volte guild for support. Join the guild [here](https://greemdev.net/Discord).")
                .SendTo(Context.Channel);
        }
    }
}