using System;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Commands.TypeParsers;
using Volte.Core.Services;
using Volte.Core.Data;
using Volte.Core.Extensions;

namespace Volte.Core.Discord {
    #pragma warning disable 1998
    public class VolteBot {
        public static readonly IServiceProvider ServiceProvider = BuildServiceProvider();

        public static readonly CommandService CommandService = ServiceProvider.GetRequiredService<CommandService>();

        public static readonly DiscordSocketClient Client = ServiceProvider.GetRequiredService<DiscordSocketClient>();

        private static readonly VolteHandler Handler = new VolteHandler();
        private static readonly LoggingService Logger = ServiceProvider.GetRequiredService<LoggingService>();

        /// <summary>
        ///     WARNING:
        ///     Instantiating this object will start a completely new bot instance.
        ///     Don't do that, unless you're making a restart function!
        /// </summary>

        public static void Start() {
            Logger.PrintVersion().GetAwaiter().GetResult();
            new VolteBot().LoginAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static IServiceProvider BuildServiceProvider() {
            return new ServiceCollection()
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
                .AddSingleton<PingChecksService>()
                .AddSingleton<LoggingService>()
                .AddSingleton<GuildService>()
                .AddSingleton(new CommandService(new CommandServiceConfiguration {
                    IgnoreExtraArguments = true,
                    CaseSensitive = false,
                    DefaultRunMode = RunMode.Sequential,
                    SeparatorRequirement = SeparatorRequirement.SeparatorOrWhitespace,
                    NullableNouns = null
                }))
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig {
                    LogLevel = LogSeverity.Verbose,
                    AlwaysDownloadUsers = true,
                    ConnectionTimeout = 10000,
                    MessageCacheSize = 100
                })).BuildServiceProvider();
        }

        private async Task LoginAsync() {
            Initialize();
            if (string.IsNullOrEmpty(Config.GetToken()) || Config.GetToken().EqualsIgnoreCase("token here")) return;
            await Client.LoginAsync(TokenType.Bot, Config.GetToken());
            await Client.StartAsync();
            if (Config.GetStreamer().EqualsIgnoreCase("streamer here")) {
                await Client.SetGameAsync(Config.GetGame());
            }
            else {
                await Client.SetGameAsync(Config.GetGame(), $"https://twitch.tv/{Config.GetStreamer()}",
                    ActivityType.Streaming);
            }
            await Client.SetStatusAsync(UserStatus.Online);
            await Handler.Init();
            Client.Log += async m => await ServiceProvider.GetRequiredService<LoggingService>().Log(m);
            await Task.Delay(-1);
        }

        private void Initialize() {
            CommandService.AddTypeParser(new UserParser<SocketGuildUser>());
            CommandService.AddTypeParser(new UserParser<SocketUser>());
            CommandService.AddTypeParser(new RoleParser<SocketRole>());
            CommandService.AddTypeParser(new ChannelParser<SocketTextChannel>());
        }
    }
}