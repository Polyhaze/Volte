using System.Threading.Tasks;
using Discord.Commands;
using Volte.Core.Discord;
using Volte.Helpers;

namespace Volte.Core.Modules.Utility {
    public partial class UtilityModule : VolteModule {
        [Command("Ping")]
        [Summary("Show the Gateway latency to Discord.")]
        public async Task Ping() {
            await Reply(Context.Channel,
                CreateEmbed(Context, $"{VolteBot.Client.Latency}ms"));
        }
    }
}