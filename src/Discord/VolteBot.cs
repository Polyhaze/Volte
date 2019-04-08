using System;
using System.IO;
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
        public static readonly CancellationTokenSource Cts = new CancellationTokenSource();
        private readonly VolteHandler _handler = GetRequiredService<VolteHandler>();
        private readonly LoggingService _logger = GetRequiredService<LoggingService>();
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
                Cts.Cancel();
            };
        }

        private async Task LoginAsync()
        {
            Console.Title = "Volte";
            Console.CursorVisible = false;
            if (!Directory.Exists("data"))
            {
                await _logger.Log(LogSeverity.Critical, LogSource.Volte,
                    "The \"data\" directory didn't exist, so I created it for you.");
                Directory.CreateDirectory("data");
                return;
            }

            if (Config.Token.IsNullOrEmpty() || Config.Token.EqualsIgnoreCase("token here")) return;
            await Client.LoginAsync(TokenType.Bot, Config.Token);
            await Client.StartAsync();

            await Client.SetStatusAsync(UserStatus.Online);
            await _handler.InitAsync();
            try
            {
                await Task.Delay(-1, Cts.Token);
            }
            catch (TaskCanceledException)
            {
                //this exception should occur, so put the shutdown logic inside the catch block
                await ShutdownAsync();
            }
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
            Cts.Dispose();
            ServiceProvider.Dispose();
            Client.Dispose();
        }
    }
}