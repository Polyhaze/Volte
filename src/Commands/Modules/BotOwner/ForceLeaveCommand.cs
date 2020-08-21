using System.Threading.Tasks;
using DSharpPlus.Entities;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("ForceLeave")]
        [Description("Forcefully leaves the guild with the given name.")]
        [Remarks("forceleave {Guild}")]
        public async Task<ActionResult> ForceLeaveAsync([Remainder] DiscordGuild guild)
        {
            await guild.LeaveAsync();
            return Ok($"Successfully left **{guild.Name}**.");
        }
    }
}