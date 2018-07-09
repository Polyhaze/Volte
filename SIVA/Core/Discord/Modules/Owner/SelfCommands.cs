using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Modules.Owner
{
    public class SelfCommands : SIVACommand
    {
        [Command("SetGame")]
        public async Task SetGame([Remainder]string game)
        {
            if (!Utils.IsBotOwner(Context.User))
            {
                await Context.Message.AddReactionAsync(new Emoji("❌"));
                return;
            }
            await DiscordLogin.Client.SetGameAsync(game);
            await Context.Channel.SendMessageAsync("", false,
                Utils.CreateEmbed(Context, $"Set the bot's game to **{game}**."));
        }

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

        [Command("SetStatus")]
        public async Task SetStatus([Remainder]string status)
        {
            var embed = new EmbedBuilder();
            var config = ServerConfig.Get(Context.Guild);
            embed.WithAuthor(Context.User);
            embed.WithColor(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB);
            switch (status.ToLower())
            {
                case "dnd":
                    await DiscordLogin.Client.SetStatusAsync(UserStatus.DoNotDisturb);
                    embed.WithDescription("Set the status to Do Not Disturb.");
                    break;
                case "idle":
                    await DiscordLogin.Client.SetStatusAsync(UserStatus.Idle);
                    embed.WithDescription("Set the status to Idle.");
                    break;
                case "invisible":
                    await DiscordLogin.Client.SetStatusAsync(UserStatus.Invisible);
                    embed.WithDescription("Set the status to Invisible.");
                    break;
                case "online":
                    await DiscordLogin.Client.SetStatusAsync(UserStatus.Online);
                    embed.WithDescription("Set the status to Online.");
                    break;
                default:
                    await DiscordLogin.Client.SetStatusAsync(UserStatus.Online);
                    embed.WithDescription(
                        "Your option wasn't known, so I set the status to Online.\nAvailable options for this command are `dnd`, `idle`, `invisible`, or `online`.");
                    break;
            }
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}