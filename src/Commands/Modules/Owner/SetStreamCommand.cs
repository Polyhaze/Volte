using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Discord;
using Volte.Extensions;

namespace Volte.Commands.Modules.Owner
{
    public partial class OwnerModule : VolteModule
    {
        [Command("SetStream")]
        [Description("Sets the bot's stream.")]
        [Remarks("Usage: |prefix|setstream {streamer} {streamName}")]
        [RequireBotOwner]
        public async Task SetStreamAsync(string streamer, [Remainder] string streamName)
        {
            await Context.Client.SetGameAsync(streamName, $"https://twitch.tv/{streamer}", ActivityType.Streaming);
            await Context
                .CreateEmbed(
                    $"Set the bot's stream to **{streamName}**, and the Twitch URL to **[{streamer}](https://twitch.tv/{streamer})**.")
                .SendToAsync(Context.Channel);
        }
    }
}