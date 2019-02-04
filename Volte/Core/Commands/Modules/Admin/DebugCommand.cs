using System.Threading.Tasks;
using Discord.Commands;
using Newtonsoft.Json;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Services;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Admin {
    public partial class AdminModule : VolteModule {
        
        public DebugService DebugService { get; set; }
        
        [Command("Debug")]
        [Summary("Generates a debug report for this guild.")]
        [Remarks("Usage: |prefix|debug")]
        [RequireGuildAdmin]
        public async Task Debug() {
            await Reply(Context.Channel,
                CreateEmbed(Context,
                    $"{DebugService.Execute(JsonConvert.SerializeObject(Db.GetConfig(Context.Guild), Formatting.Indented))}" +
                    "\n\nTake this to the Volte guild for support. Join the guild [here](https://greemdev.net/discord)."));
        }
    }
}