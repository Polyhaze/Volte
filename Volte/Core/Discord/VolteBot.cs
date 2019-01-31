using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Services;
using Volte.Core.Files.Readers;
using Volte.Core.Runtime;

namespace Volte.Core.Discord {
    #pragma warning disable 1998
    public class VolteBot {
        public static readonly IServiceProvider ServiceProvider = BuildServiceProvider();

        public static readonly CommandService CommandService = ServiceProvider.GetRequiredService<CommandService>();

        public static readonly DiscordSocketClient Client = ServiceProvider.GetRequiredService<DiscordSocketClient>();

        private static readonly VolteHandler Handler = new VolteHandler();
        private static readonly Logger Logger = new Logger();

        /// <summary>
        ///     WARNING:
        ///     Instantiating this object will start a completely new bot instance.
        ///     Don't do that, unless you're making a restart function!
        /// </summary>
        public VolteBot() {
            Logger.PrintVersion();
            LoginAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static IServiceProvider BuildServiceProvider() {
            var c = new ServiceCollection()
                .AddSingleton<AntilinkService>()
                .AddSingleton<AutoroleService>()
                .AddSingleton<BlacklistService>()
                .AddSingleton<EconomyService>()
                .AddSingleton<WelcomeService>()
                .AddSingleton<DatabaseService>()
                .AddSingleton<VolteHandler>()
                .AddSingleton<EventService>()
                .AddSingleton<DebugService>()
                .AddSingleton<EmojiService>()
                .AddSingleton(new CommandService(new CommandServiceConfig {
                    IgnoreExtraArgs = true,
                    DefaultRunMode = RunMode.Async,
                    CaseSensitiveCommands = false,
                    LogLevel = LogSeverity.Verbose,
                    ThrowOnError = false
                }))
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig {
                    LogLevel = LogSeverity.Verbose,
                    AlwaysDownloadUsers = true,
                    ConnectionTimeout = 10000,
                    MessageCacheSize = 100
                }));

            return c.BuildServiceProvider();
        }

        private async Task LoginAsync() {
            await Client.LoginAsync(TokenType.Bot, Config.GetToken());
            await Client.StartAsync();
            await Client.SetGameAsync(Config.GetGame(), $"https://twitch.tv/{Config.GetStreamer()}",
                ActivityType.Streaming);
            await Client.SetStatusAsync(UserStatus.Online);
            await Handler.Init();
            Client.Log += Log;
            await Task.Delay(-1);
        }

        private static async Task Log(LogMessage msg) {
            switch (msg.Severity) {
                case LogSeverity.Info:
                case LogSeverity.Verbose:
                    Logger.Info(msg.Message);
                    break;
                case LogSeverity.Warning:
                    Logger.Warn(msg.Message);
                    break;
                case LogSeverity.Error:
                    Logger.Error(msg.Message);
                    break;
                case LogSeverity.Critical:
                    Logger.Error(msg.Message);
                    break;
                case LogSeverity.Debug:
                    Logger.Debug(msg.Message);
                    break;
                default:
                    throw new InvalidDataException();
            }
        }
    }
}