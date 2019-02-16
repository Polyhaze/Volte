using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Discord;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Owner
{
    public partial class OwnerModule : VolteModule
    {
        [Command("ForceDebug")]
        [Description("Forces a debug report for the guild with the given ID.")]
        [Remarks("Usage: $forcedebug {guildId}")]
        [RequireBotOwner]
        public async Task ForceDebug(ulong serverId)
        {
            await Context.CreateEmbed(
                    DebugService.Execute(JsonConvert.SerializeObject(Db.GetConfig(VolteBot.Client.GetGuild(serverId)),
                        Formatting.Indented)))
                .SendTo(Context.Channel);
        }
    }
}