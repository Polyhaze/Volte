using System.Linq;
using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Discord;
using Volte.Extensions;

namespace Volte.Commands.Modules.Owner
{
    public partial class OwnerModule : VolteModule
    {
        [Command("ForceLeave")]
        [Description("Forcefully leaves the guild with the given name.")]
        [Remarks("Usage: |prefix|forceleave {serverName}")]
        [RequireBotOwner]
        public async Task ForceLeaveAsync([Remainder] string serverName)
        {
            var target = VolteBot.Client.Guilds.FirstOrDefault(g => g.Name == serverName);
            if (target is null)
            {
                await Context.CreateEmbed($"I'm not in the guild **{serverName}**.").SendToAsync(Context.Channel);
                return;
            }

            await target.LeaveAsync();
            await Context.CreateEmbed($"Successfully left **{target.Name}**").SendToAsync(Context.Channel);
        }
    }
}