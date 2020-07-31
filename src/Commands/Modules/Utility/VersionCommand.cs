using System.Threading.Tasks;

using Discord.Commands;

namespace BrackeysBot.Commands
{
    public partial class UtilityModule : BrackeysBotModule
    {
        [Command("version"), Alias("v")]
        [Summary("Displays the current version of the bot.")]
        public async Task ShowVersionAsync()
        {
            await GetDefaultBuilder()
                .WithDescription($"This is **BrackeysBot v{Version.FullVersion}** running **Discord.Net v{Version.DiscordVersion}**!")
                .Build()
                .SendToChannel(Context.Channel);
        }
    }
}
