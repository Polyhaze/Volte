using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class BotOwnerModule : VolteModule
    {
        [Command("SetStream")]
        [Description("Sets the bot's stream.")]
        [Remarks("Usage: |prefix|setstream {streamer} {streamName}")]
        [RequireBotOwner]
        public async Task<ActionResult> SetStreamAsync(string streamer, [Remainder] string streamName)
            => Ok(
                $"Set the bot's stream to **{streamName}**, and the Twitch URL to **[{streamer}](https://twitch.tv/{streamer})**.",
                _ => Context.Client.SetGameAsync(streamName, $"https://twitch.tv/{streamer}", ActivityType.Streaming));
    }
}