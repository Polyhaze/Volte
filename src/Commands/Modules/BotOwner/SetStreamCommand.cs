using Discord;
using Gommon;
using Qmmands;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Volte.Commands.Results;
using Volte.Core.Entities;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("SetStream")]
        [Description("Sets the bot's stream via Twitch username and Stream name, respectively.")]
        public Task<ActionResult> SetStreamAsync([Description("The Twitch username to link to in the status.")] string stream, [Remainder, Description("The stream title to show.")] string game = null)
        {
            return !game.IsNullOrWhitespace()
                ? Ok(
                        $"Set the bot's game to **{game}**, and the Twitch URL to **[{stream}](https://twitch.tv/{stream})**.",
                        _ => Context.Client.SetGameAsync(game, $"https://twitch.tv/{stream}", ActivityType.Streaming))
                : Ok(
                       $"Set the bot's stream URL to **[{stream}](https://twitch.tv/{stream})**.",
                       _ => Context.Client.SetGameAsync(Context.Client.Activity.Name, $"https://twitch.tv/{stream}", ActivityType.Streaming));
        }
    }
}
