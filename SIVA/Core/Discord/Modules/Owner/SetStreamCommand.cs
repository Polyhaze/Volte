using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using SIVA.Helpers;
using System.Linq;

namespace SIVA.Core.Discord.Modules.Owner {
    public class SetStreamCommand : SIVACommand {
        [Command("SetStream")]
        public async Task SetStream(string twitchUrl, [Remainder] string streamName) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await React(Context.SMessage, RawEmoji.X);
                return;
            }

            var twitchStreamer = twitchUrl.Split(".tv/").ToList().Last();
            await SIVA.Client.SetGameAsync(streamName, twitchUrl, ActivityType.Streaming);
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context,
                    $"Set the bot's stream to **{streamName}**, and the twitch URL to [{twitchStreamer}]({twitchUrl})."));
        }
    }
}