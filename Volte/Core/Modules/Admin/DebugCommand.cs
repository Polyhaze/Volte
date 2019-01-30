using System.Threading.Tasks;
using Discord.Commands;
using Newtonsoft.Json;
using Volte.Core.Services;
using Volte.Helpers;

namespace Volte.Core.Modules.Admin {
    public partial class AdminModule : VolteModule {
        
        public DebugService DebugService { get; set; }
        
        [Command("Debug")]
        [Summary("Generates a debug report for this guild.")]
        [Remarks("Usage: |prefix|debug")]
        public async Task Debug() {
            if (!UserUtils.IsAdmin(Context)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }
            
            await Reply(Context.Channel,
                CreateEmbed(Context,
                    $"{DebugService.Execute(JsonConvert.SerializeObject(Db.GetConfig(Context.Guild)))}" +
                    "\n\nTake this to the Volte guild for support. Join the guild [here](https://greemdev.net/discord)."));
        }
    }
}