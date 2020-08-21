using Discord;
using Gommon;
using Qmmands;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("SetStream")]
        [Description("Sets the bot's stream via Twitch username and Stream name, respectively.")]
        [Remarks("setstream {String} [String]")]
        public Task<ActionResult> SetStreamAsync(string stream, [Remainder] string game = null)
        {
            return !game.IsNullOrWhitespace()
                ? Ok(
                        $"Set the bot's game to **{game}**, and the Twitch URL to **[{stream}](https://twitch.tv/{stream})**.",
                        _ => Context.Client.UpdateStatusAsync(new DiscordActivity(game, ActivityType.Streaming) { StreamUrl = $"https://twitch.tv/{stream}" }))
                : Ok(
                       $"Set the bot's stream URL to **[{stream}](https://twitch.tv/{stream})**.",
                       _ => Context.Client.SetGameAsync(Context.Client.Activity.Name, $"https://twitch.tv/{stream}", ActivityType.Streaming)); // TODO same issue as SetGameCommand
        }
    }
}
