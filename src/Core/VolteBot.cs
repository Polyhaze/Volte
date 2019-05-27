using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using RestSharp;
using Volte.Data;
using Volte.Data.Models;
using Volte.Extensions;
using Volte.Services;

namespace Volte.Core
{
    public class VolteBot : IDisposable
    {
        public static readonly ServiceProvider ServiceProvider = BuildServiceProvider();
        public static readonly CommandService CommandService = GetRequiredService<CommandService>();
        public static readonly DiscordSocketClient Client = GetRequiredService<DiscordSocketClient>();
        public static readonly CancellationTokenSource Cts = new CancellationTokenSource();
        private readonly VolteHandler _handler = VolteHandler.Instance;
        private readonly LoggingService _logger = LoggingService.Instance;

        public static T GetRequiredService<T>() => ServiceProvider.GetRequiredService<T>();

        public static Task StartAsync()
            => new VolteBot().LoginAsync();

        private static ServiceProvider BuildServiceProvider()
            => new ServiceCollection()
                .AddSingleton<VolteHandler>()
                .AddSingleton(new RestClient { UserAgent = $"Volte/{Version.FullVersion}" })
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
                    LogLevel = Version.ReleaseType is ReleaseType.Release
                        ? LogSeverity.Verbose
                        : LogSeverity.Debug,
                    AlwaysDownloadUsers = true,
                    ConnectionTimeout = 10000,
                    MessageCacheSize = 50
                }))
                .AddVolteServices()
                .BuildServiceProvider();

        private VolteBot()
            => Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                Cts.Cancel();
            };

        private async Task LoginAsync()
        {
            Console.Title = "Volte";
            Console.CursorVisible = false;
            if (!Directory.Exists("data"))
            {
                await _logger.LogAsync(LogSeverity.Critical, LogSource.Volte,
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