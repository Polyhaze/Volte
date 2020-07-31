using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BrackeysBot.Commands
{
    public partial class UtilityModule : BrackeysBotModule
    {
        [Command("ping"), Alias("latency")]
        [Summary("Displays the current latency of the bot.")]
        public async Task GetLatencyAsync()
        {
            int latency = (Context.Client as DiscordSocketClient).Latency;

            await GetDefaultBuilder()
                .WithDescription($"Current latency is about {latency}ms.")
                .Build()
                .SendToChannel(Context.Channel);
        }
    }
}
