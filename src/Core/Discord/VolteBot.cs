using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Volte.Core.Data;
using Volte.Core.Extensions;
using Volte.Core.Runtime;
using Volte.Core.Services;

namespace Volte.Core.Discord
{
#pragma warning disable 1998
    public class VolteBot
    {
        public static readonly IServiceProvider ServiceProvider = BuildServiceProvider();

        public static readonly CommandService CommandService = GetRequiredService<CommandService>();

        public static readonly DiscordSocketClient Client = GetRequiredService<DiscordSocketClient>();

        private readonly VolteHandler _handler = GetRequiredService<VolteHandler>();

        public static T GetRequiredService<T>()
        {
            return ServiceProvider.GetRequiredService<T>();
        }

        public static async Task StartAsync()
        {
            await GetRequiredService<LoggingService>().PrintVersion();
            await new VolteBot().LoginAsync();
        }

        private static IServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton<VolteHandler>()
                .AddVolteServices()
                .AddSingleton(new CommandService(new CommandServiceConfiguration
                {
                    IgnoreExtraArguments = true,
                    CaseSensitive = false,
                    DefaultRunMode = RunMode.Sequential,
                    SeparatorRequirement = SeparatorRequirement.Separator,
                    Separator = "irrelevant",
                    NullableNouns = null
                }))
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = Runtime.Version.ReleaseType != ReleaseType.Release
                        ? LogSeverity.Debug
                        : LogSeverity.Verbose,
                    AlwaysDownloadUsers = true,
                    ConnectionTimeout = 10000,
                    MessageCacheSize = 50
                })).BuildServiceProvider();
        }

        private async Task LoginAsync()
        {
            CommandService.AddTypeParsers();

            if (Config.Token.IsNullOrEmpty() || Config.Token.EqualsIgnoreCase("token here")) return;
            await Client.LoginAsync(TokenType.Bot, Config.Token);
            await Client.StartAsync();
            if (Config.Streamer.EqualsIgnoreCase("streamer here") ||
                Config.Streamer.IsNullOrWhitespace())
                await Client.SetGameAsync(Config.Game);
            else
                await Client.SetGameAsync(Config.Game,
                    $"https://twitch.tv/{Config.Streamer}",
                    ActivityType.Streaming);

            await Client.SetStatusAsync(UserStatus.Online);
            await _handler.InitAsync();
            await Task.Delay(-1);
        }
    }
}