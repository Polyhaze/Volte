using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.Owner
{
    public partial class OwnerModule : VolteModule
    {
        [Command("ForceDebug")]
        [Description("Forces a debug report for the guild with the given ID.")]
        [Remarks("Usage: |prefix|forcedebug {guildId}")]
        [RequireBotOwner]
        public async Task ForceDebugAsync(ulong serverId)
        {
            await Context.CreateEmbed(
                    DebugService.Execute(Db.GetConfig(serverId).ToString()))
                .SendToAsync(Context.Channel);
        }
    }
}