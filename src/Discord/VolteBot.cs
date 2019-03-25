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
    public class VolteBot : IDisposable
    {
        public static readonly ServiceProvider ServiceProvider = BuildServiceProvider();
        public static readonly CommandService CommandService = GetRequiredService<CommandService>();
        public static readonly DiscordSocketClient Client = GetRequiredService<DiscordSocketClient>();
        public static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        private readonly VolteHandler _handler = GetRequiredService<VolteHandler>();
        public static T GetRequiredService<T>() => ServiceProvider.GetRequiredService<T>();

        public static Task StartAsync()
            => new VolteBot().LoginAsync();

        private static ServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton<VolteHandler>()
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

        private VolteBot()
        {
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                CancellationTokenSource.Cancel();
            };
        }

        private async Task LoginAsync()
        {
            CommandService.AddTypeParsers();

            if (Config.Token.IsNullOrEmpty() || Config.Token.EqualsIgnoreCase("token here")) return;
            await Client.LoginAsync(TokenType.Bot, Config.Token);
            await Client.StartAsync();

            await Client.SetStatusAsync(UserStatus.Online);
            await _handler.InitAsync();
            try
            {
                await Task.Delay(-1, CancellationTokenSource.Token);
            }
            catch (TaskCanceledException) { } //this should happen, so w/e

            await ShutdownAsync();
        }

        private async Task ShutdownAsync()
        {
            await Client.SetStatusAsync(UserStatus.Invisible);
            await Client.LogoutAsync();
            await Client.StopAsync();
            Dispose();
            Environment.Exit(0);
        }

        public void Dispose()
        {
            CancellationTokenSource.Dispose();
            ServiceProvider.Dispose();
            Client.Dispose();
        }
    }
}