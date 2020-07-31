using System;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

namespace BrackeysBot.Services
{
    public class StartupService : BrackeysBotService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _discord;
        private readonly DataService _data;
        private readonly LoggingService _log;

        public StartupService(IServiceProvider provider, DiscordSocketClient discord, DataService data, LoggingService log)
        {
            _provider = provider;
            _discord = discord;
            _data = data;
            _log = log;

            _discord.Ready += DisplayStartupVersionAsync;
        }

        public async Task StartAsync()
        {
            string discordToken = _data.Configuration.Token;
            if (string.IsNullOrEmpty(discordToken))
            {
                await _log.LogMessageAsync(new LogMessage(LogSeverity.Error, "Startup", "The login token in the config.yaml file was not set."));
                return;
            }

            await _discord.LoginAsync(TokenType.Bot, discordToken);
            await _discord.StartAsync();
        }

        private async Task DisplayStartupVersionAsync()
        {
            ulong guild = _data.Configuration.GuildID;
            ulong logChannel = _data.Configuration.ModerationLogChannelID;

            if (guild == 0 || logChannel == 0)
                return;

            ITextChannel channel = _discord.GetGuild(guild).GetTextChannel(logChannel);
            await channel.SendMessageAsync(string.Empty, false, new EmbedBuilder()
                .WithColor(Color.Green)
                .WithTitle("Startup complete!")
                .WithDescription($"This is **BrackeysBot v{Version.FullVersion}** running **Discord.Net v{Version.DiscordVersion}**!")
                .Build());
        }
    }
}