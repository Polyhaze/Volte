using System.Threading.Tasks;
using Discord.Commands;
using SIVA.Helpers;

namespace SIVA.Core.Modules.Information.Utilities {
    public class PingCommand : SIVACommand {
        [Command("Ping")]
        public async Task Ping() {
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"{Discord.SIVA.Client.Latency}ms"));
        }
    }
}