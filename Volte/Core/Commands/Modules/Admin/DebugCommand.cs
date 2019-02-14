using System.Threading.Tasks;
using Qmmands;
using Newtonsoft.Json;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;
using Volte.Core.Services;

namespace Volte.Core.Commands.Modules.Admin {
    public partial class AdminModule : VolteModule {
        
        public DebugService DebugService { get; set; }
        
        [Command("Debug")]
        [Description("Generates a debug report for this guild.")]
        [Remarks("Usage: |prefix|debug")]
        [RequireGuildAdmin]
        public async Task Debug() {
            await Context.CreateEmbed(
                    $"{DebugService.Execute(JsonConvert.SerializeObject(Db.GetConfig(Context.Guild), Formatting.Indented))}" +
                    "\n\nTake this to the Volte guild for support. Join the guild [here](https://greemdev.net/discord).")
                .SendTo(Context.Channel);
        }
    }
}