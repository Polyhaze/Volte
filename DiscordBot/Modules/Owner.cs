using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace SIVA.Modules
{
    public class Owner : ModuleBase<SocketCommandContext>
    {

        [Command("Shutdown")]
        [RequireOwner]
        public async Task Shutdown()
        {

            var _client = new DiscordSocketClient();

            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetFormattedAlert("LoggingOutMsg", Context.User.Mention));
            embed.WithColor(Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            await Context.Channel.SendMessageAsync("", false, embed);
            await _client.LogoutAsync();
            await _client.StopAsync();

        }

        /*[Command("Blacklist"), Alias("Bl")]
        [RequireOwner]
        public async Task AddUserToBlacklist(SocketUser user)
        {
            var config = Config.bot.BlacklistedUsers;
            config.Append(user);
            var embed = new EmbedBuilder();
            embed.WithDescription($"Successfully added {user.Mention} to the blacklist.");
            embed.WithColor(Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));

            await Context.Channel.SendMessageAsync("", false, embed);
        }*/

        [Command("Game")]
        [RequireOwner]
        public async Task SetBotGame([Remainder] string game)
        {
            var client = new DiscordSocketClient();

            var embed = new EmbedBuilder();
            embed.WithDescription($"Set the bot's game to {game}");
            embed.WithColor(Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            await client.SetGameAsync(game);
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("Status")]
        [RequireOwner]
        public async Task SetBotStatus(string status)
        {
            var embed = new EmbedBuilder();
            embed.WithDescription($"Set the status to {status}.");
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithColor(Config.bot.DefaultEmbedColour);

            var client = new DiscordSocketClient();

            switch (status)
            {
                case "dnd":
                    await client.SetStatusAsync(UserStatus.DoNotDisturb);
                    break;
                case "idle":
                    await client.SetStatusAsync(UserStatus.Idle);
                    break;
                case "online":
                    await client.SetStatusAsync(UserStatus.Online);
                    break;
                case "offline":
                    await client.SetStatusAsync(UserStatus.Invisible);
                    break;
            }

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("LeaveServer")]
        [RequireOwner]
        public async Task LeaveServer()
        {
            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetAlert("BotLeftServer"));
            embed.WithColor(Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            await Context.Channel.SendMessageAsync("", false, embed);
            await Context.Guild.LeaveAsync();
        }

        [Command("ServerCount"), Alias("Sc")]
        [RequireOwner]
        public async Task ServerCountStream()
        {
            var client = new DiscordSocketClient();
            var guilds = Context.Client.Guilds.Count;
            var embed = new EmbedBuilder();
            embed.WithDescription("Done.");
            embed.WithColor(Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            await Context.Channel.SendMessageAsync("", false, embed);
            await client.SetGameAsync($"in {(Context.Client as DiscordSocketClient).Guilds.Count} servers!", $"https://twitch.tv/{Config.bot.TwitchStreamer}", StreamType.Twitch);

        }
    }
}
