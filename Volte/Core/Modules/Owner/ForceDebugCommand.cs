using System.Threading.Tasks;
using Discord.Commands;
using Newtonsoft.Json;
using Volte.Core.Services;
using Volte.Helpers;

namespace Volte.Core.Modules.Owner {
    public partial class OwnerModule : VolteModule {
        
        public DebugService DebugService { get; set; }
        
        [Command("ForceDebug")]
        [Summary("Forces a debug report for the guild with the given ID.")]
        [Remarks("Usage: $forcedebug {guildId}")]
        public async Task ForceDebug(ulong serverId) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            await Reply(Context.Channel,
                CreateEmbed(Context,
                    DebugService.Execute(JsonConvert.SerializeObject(Db.GetConfig(Context.Guild))
                    )
                ));
        }
    }
}