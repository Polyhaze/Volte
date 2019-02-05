using System.Threading.Tasks;
using Discord.Commands;
using Newtonsoft.Json;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Extensions;
using Volte.Core.Services;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Owner {
    public partial class OwnerModule : VolteModule {
        
        public DebugService DebugService { get; set; }
        
        [Command("ForceDebug")]
        [Summary("Forces a debug report for the guild with the given ID.")]
        [Remarks("Usage: $forcedebug {guildId}")]
        [RequireBotOwner]
        public async Task ForceDebug(ulong serverId) {
            await Context.CreateEmbed(
                    DebugService.Execute(JsonConvert.SerializeObject(Db.GetConfig(Context.Guild), Formatting.Indented)))
                .SendTo(Context.Channel);
        }
    }
}