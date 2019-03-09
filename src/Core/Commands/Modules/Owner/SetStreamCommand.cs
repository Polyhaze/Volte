using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Commands.Preconditions;
using Volte.Core.Discord;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Owner
{
    public partial class OwnerModule : VolteModule
    {
        [Command("SetStream")]
        [Description("Sets the bot's stream.")]
        [Remarks("Usage: |prefix|setstream {streamer} {streamName}")]
        [RequireBotOwner]
        public async Task SetStreamAsync(string streamer, [Remainder] string streamName)
        {
            await VolteBot.Client.SetGameAsync(streamName, $"https://twitch.tv/{streamer}", ActivityType.Streaming);
            await Context
                .CreateEmbed(
                    $"Set the bot's stream to **{streamName}**, and the Twitch URL to **[{streamer}](https://twitch.tv/{streamer})**.")
                .SendToAsync(Context.Channel);
        }
    }
}