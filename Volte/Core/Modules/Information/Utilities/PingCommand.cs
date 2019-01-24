using System.Threading.Tasks;
using Discord.Commands;
using Volte.Core.Discord;
using Volte.Helpers;

namespace Volte.Core.Modules.Information.Utilities {
    public class PingCommand : VolteCommand {
        [Command("Ping")]
        public async Task Ping() {
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"{VolteBot.Client.Latency}ms"));
        }
    }
}