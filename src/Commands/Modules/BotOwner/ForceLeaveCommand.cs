using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule : VolteModule
    {
        [Command("ForceLeave")]
        [Description("Forcefully leaves the guild with the given name.")]
        [Remarks("Usage: |prefix|forceleave {guild}")]
        [RequireBotOwner]
        public async Task<ActionResult> ForceLeaveAsync([Remainder] SocketGuild guild)
        {
            await guild.LeaveAsync();
            return Ok($"Successfully left **{guild.Name}**.");
        }
    }
}