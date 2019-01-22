using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using SIVA.Helpers;
using System.Linq;

namespace SIVA.Core.Modules.Owner {
    public class SetStreamCommand : SIVACommand {
        [Command("SetStream")]
        public async Task SetStream(string streamer, [Remainder] string streamName) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            await Discord.SIVA.Client.SetGameAsync(streamName, $"https://twitch.tv/{streamer}", ActivityType.Streaming);
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context,
                    $"Set the bot's stream to **{streamName}**, and the twitch URL to **[{streamer}](https://twitch.tv/{streamer})**."));
        }
    }
}