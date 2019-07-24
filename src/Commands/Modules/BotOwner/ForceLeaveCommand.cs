using System.Linq;
using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class BotOwnerModule : VolteModule
    {
        [Command("ForceLeave")]
        [Description("Forcefully leaves the guild with the given name.")]
        [Remarks("Usage: |prefix|forceleave {serverName}")]
        [RequireBotOwner]
        public async Task<ActionResult> ForceLeaveAsync([Remainder] string serverName)
        {
            var target = Context.Client.Guilds.FirstOrDefault(g => g.Name == serverName);
            if (target is null)
            {
                return BadRequest($"I'm not in the guild **{serverName}**.");
            }

            await target.LeaveAsync();
            return Ok($"Successfully left **{target.Name}**");
        }
    }
}