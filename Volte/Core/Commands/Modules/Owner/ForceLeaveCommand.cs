using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Discord;
using Volte.Helpers;

namespace Volte.Core.Commands.Modules.Owner {
    public partial class OwnerModule : VolteModule {
        [Command("ForceLeave")]
        [Summary("Forcefully leaves the guild with the given name.")]
        [Remarks("Usage: $forceleave {serverName}")]
        [RequireBotOwner]
        public async Task ForceLeave([Remainder]string serverName) {
            var target = VolteBot.Client.Guilds.FirstOrDefault(g => g.Name == serverName);
            if (target is null) {
                await Context.Channel.SendMessageAsync(string.Empty, false,
                    CreateEmbed(Context, $"I'm not in the guild **{serverName}**."));
                return;
            }

            await target.LeaveAsync();
            await Context.Channel.SendMessageAsync(string.Empty, false,
                CreateEmbed(Context, $"Successfully left **{target.Name}**"));
        }
    }
}