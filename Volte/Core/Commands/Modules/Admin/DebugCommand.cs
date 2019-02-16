using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;
using Volte.Core.Services;

namespace Volte.Core.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("Debug")]
        [Description("Generates a debug report for this guild.")]
        [Remarks("Usage: |prefix|debug")]
        [RequireGuildAdmin]
        public async Task DebugReport()
        {
            await Context.CreateEmbed(
                    $"{DebugService.Execute(JsonConvert.SerializeObject(Db.GetConfig(Context.Guild), Formatting.Indented))}" +
                    "\n\nTake this to the Volte guild for support. Join the guild [here](https://greemdev.net/discord).")
                .SendTo(Context.Channel);
        }
    }
}