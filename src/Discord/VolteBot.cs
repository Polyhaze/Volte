using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Volte.Data;
using Volte.Data.Objects;
using Volte.Extensions;
using Volte.Services;

namespace Volte.Discord
{
    public class VolteBot
    {
        public static readonly IServiceProvider ServiceProvider = BuildServiceProvider();

        public static readonly CommandService CommandService = GetRequiredService<CommandService>();

        public static readonly DiscordSocketClient Client = GetRequiredService<DiscordSocketClient>();

        private readonly VolteHandler _handler = GetRequiredService<VolteHandler>();

        public static T GetRequiredService<T>() => ServiceProvider.GetRequiredService<T>();

        public static async Task StartAsync()
        {
            await new VolteBot().LoginAsync();
        }

        private static IServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton<VolteHandler>()
                .AddSingleton(new CancellationTokenSource())
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
                    LogLevel = Version.ReleaseType != ReleaseType.Release
                        ? LogSeverity.Debug
                        : LogSeverity.Verbose,
                    AlwaysDownloadUsers = true,
                    ConnectionTimeout = 10000,
                    MessageCacheSize = 50
                }))
                .AddVolteServices()
                .BuildServiceProvider();
        }

        private async Task LoginAsync()
        {
            CommandService.AddTypeParsers();

            if (Config.Token.IsNullOrEmpty() || Config.Token.EqualsIgnoreCase("token here")) return;
            await Client.LoginAsync(TokenType.Bot, Config.Token);
            await Client.StartAsync();

            await Client.SetStatusAsync(UserStatus.Online);
            await _handler.InitAsync();
            await Task.Delay(-1, GetRequiredService<CancellationTokenSource>().Token);
        }
    }
}