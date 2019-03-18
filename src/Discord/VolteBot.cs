using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Volte.Data;
using Volte.Extensions;
using Volte.Services;

namespace Volte.Discord
{
#pragma warning disable 1998
    public class VolteBot
    {
        public static readonly IServiceProvider ServiceProvider = BuildServiceProvider();

        public static readonly CommandService CommandService = GetRequiredService<CommandService>();

        public static readonly DiscordClient Client = GetRequiredService<DiscordClient>();

        private readonly VolteHandler _handler = GetRequiredService<VolteHandler>();

        public static T GetRequiredService<T>()
            => ServiceProvider.GetRequiredService<T>();

        public static async Task StartAsync()
        {
            await GetRequiredService<LoggingService>().PrintVersion();
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
                .AddSingleton(new DiscordClient(new DiscordConfiguration
                {
                    LogLevel = Version.ReleaseType != ReleaseType.Release
                        ? LogLevel.Debug
                        : LogLevel.Info,
                    MessageCacheSize = 50,
                    Token = Config.Token,
                    TokenType = TokenType.Bot
                }))
                .AddVolteServices()
                .BuildServiceProvider();
        }

        private async Task LoginAsync()
        {
            CommandService.AddTypeParsers();

            if (Config.Token.IsNullOrEmpty() || Config.Token.EqualsIgnoreCase("token here")) return;
            await Client.ConnectAsync();
            await _handler.InitAsync();
            await Task.Delay(-1, GetRequiredService<CancellationTokenSource>().Token);
        }
    }
}