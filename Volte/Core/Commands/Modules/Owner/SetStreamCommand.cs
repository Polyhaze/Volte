using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Discord;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Owner {
    public partial class OwnerModule : VolteModule {
        [Command("SetStream")]
        [Summary("Sets the bot's stream.")]
        [Remarks("Usage: $setstream {streamer} {streamName}")]
        [RequireBotOwner]
        public async Task SetStream(string streamer, [Remainder] string streamName) {
            await VolteBot.Client.SetGameAsync(streamName, $"https://twitch.tv/{streamer}", ActivityType.Streaming);
            await Context
                .CreateEmbed(
                    $"Set the bot's stream to **{streamName}**, and the Twitch URL to **[{streamer}](https://twitch.tv/{streamer})**.")
                .SendTo(Context.Channel);
        }
    }
}