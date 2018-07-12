using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using SIVA.Helpers;
using System.Linq;

namespace SIVA.Core.Discord.Modules.Owner
{
    public class SetStreamCommand : SIVACommand
    {
        [Command("SetStream")]
        public async Task SetStream(string twitchUrl, [Remainder] string streamName)
        {
            if (!Utils.IsBotOwner(Context.User))
            {
                await Context.Message.AddReactionAsync(new Emoji("❌"));
                return;
            }
            var twitchStreamer = twitchUrl.Split(".tv/").ToList().Last();
            await DiscordLogin.Client.SetGameAsync(streamName, twitchUrl, ActivityType.Streaming);
            await Context.Channel.SendMessageAsync("", false,
                Utils.CreateEmbed(Context,
                    $"Set the bot's stream to **{streamName}**, and the twitch URL to [{twitchStreamer}]({twitchUrl})."));
        }
    }
}