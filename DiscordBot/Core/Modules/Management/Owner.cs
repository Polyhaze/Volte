using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Bot;
using SIVA.Core.JsonFiles;

namespace SIVA.Core.Modules.Management
{
    public class Owner : ModuleBase<SocketCommandContext>
    {

        [Command("Shutdown")]
        [RequireOwner]
        public async Task Shutdown()
        {
            var embed = new EmbedBuilder();
            embed.WithDescription(Bot.Utilities.GetFormattedLocaleMsg("LoggingOutMsg", Context.User.Mention));
            embed.WithColor(Bot.Config.bot.DefaultEmbedColour);
            embed.WithFooter(Bot.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            await ReplyAsync("", false, embed);
            await Context.Client.LogoutAsync();
            await Context.Client.StopAsync();
        }

        [Command("VerifyGuild"), Alias("Verify")]
        [RequireOwner]
        public async Task VerifyGuildById(ulong guildId = 0)
        {
            var id = guildId;
            if (id == 0) id = Context.Guild.Id;
            var config = GuildConfig.GetGuildConfig(id);
            config.VerifiedGuild = true;
            var embed = new EmbedBuilder()
                .WithDescription("Successfully verified this server.")
                .WithColor(Config.bot.DefaultEmbedColour)
                .WithFooter(Bot.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            await ReplyAsync("", false, embed);
        }

        /*[Command("AddDare")]
        [RequireOwner]
        public async Task AddDareToJson([Remainder]string dare)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            TruthOrDareJson.AddDareToJson(dare);
            var embed = new EmbedBuilder()
                .WithDescription("Successfully added the dare to the JSON file.")
                .AddField("Dare", dare)
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            await ReplyAsync("", false, embed);
        }

        [Command("AddTruth")]
        [RequireOwner]
        public async Task AddTruthToJson([Remainder]string truth)
        {
            var config = GuildConfig.GetOrCreateConfig(Context.Guild.Id);
            TruthOrDareJson.AddDareToJson(truth);
            var embed = new EmbedBuilder()
                .WithDescription("Successfully added the truth to the JSON file.")
                .AddField("Truth", truth)
                .WithColor(new Color(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3))
                .WithFooter(Bot.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            await ReplyAsync("", false, embed);
        }*/

        [Command("Game")]
        [RequireOwner]
        public async Task SetBotGame([Remainder] string game)
        {
            var client = new DiscordSocketClient();

            var embed = new EmbedBuilder();
            embed.WithDescription($"Set the bot's game to {game}");
            embed.WithColor(Bot.Config.bot.DefaultEmbedColour);
            embed.WithFooter(Bot.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            await client.SetGameAsync(game);
            await ReplyAsync("", false, embed);
        }

        [Command("Status")]
        [RequireOwner]
        public async Task SetBotStatus(string status)
        {
            var embed = new EmbedBuilder();
            embed.WithDescription($"Set the status to {status}.");
            embed.WithFooter(Bot.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(Bot.Config.bot.DefaultEmbedColour);

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

            await ReplyAsync("", false, embed);
        }

        [Command("LeaveServer")]
        [RequireOwner]
        public async Task LeaveServer()
        {
            var embed = new EmbedBuilder();
            embed.WithDescription(Bot.Utilities.GetLocaleMsg("BotLeftServer"));
            embed.WithColor(Bot.Config.bot.DefaultEmbedColour);
            embed.WithFooter(Bot.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            await ReplyAsync("", false, embed);
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
            embed.WithColor(Bot.Config.bot.DefaultEmbedColour);
            embed.WithFooter(Bot.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            await ReplyAsync("", false, embed);
            await client.SetGameAsync($"in {(Context.Client as DiscordSocketClient).Guilds.Count} servers!", $"https://twitch.tv/{Config.bot.TwitchStreamer}", StreamType.Twitch);

        }
    }
}
