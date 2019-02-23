using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Volte.Core.Commands.TypeParsers;
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

        private static IServiceProvider BuildServiceProvider() =>
            new ServiceCollection()
                .AddSingleton<AntilinkService>()
                .AddSingleton<AutoroleService>()
                .AddSingleton<BlacklistService>()
                .AddSingleton<WelcomeService>()
                .AddSingleton<DatabaseService>()
                .AddSingleton<VolteHandler>()
                .AddSingleton<EventService>()
                .AddSingleton<DebugService>()
                .AddSingleton<EmojiService>()
                .AddSingleton<PingChecksService>()
                .AddSingleton<LoggingService>()
                .AddSingleton<GuildService>()
                .AddSingleton(new CommandService(new CommandServiceConfiguration
                {
                    IgnoreExtraArguments = true,
                    CaseSensitive = false,
                    DefaultRunMode = RunMode.Sequential,
                    SeparatorRequirement = SeparatorRequirement.Separator,
                    Separator =
                        "not-a-space-so-we-can-use-spaces-in-command-names; nvm that doesnt work either but we dont need this separator so no one will know i'm gay",
                    NullableNouns = null
                }))
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = Runtime.Version.ReleaseType != ReleaseType.Release
                        ? LogSeverity.Debug
                        : LogSeverity.Verbose,
                    AlwaysDownloadUsers = true,
                    ConnectionTimeout = 10000,
                    MessageCacheSize = 100
                }))
                .BuildServiceProvider();

        private async Task LoginAsync()
        {
            CommandService.AddTypeParser(new UserParser<SocketGuildUser>());
            CommandService.AddTypeParser(new UserParser<SocketUser>());
            CommandService.AddTypeParser(new RoleParser<SocketRole>());
            CommandService.AddTypeParser(new ChannelParser<SocketTextChannel>());
            CommandService.AddTypeParser(new EmoteParser());
            CommandService.AddTypeParser(new BooleanParser(), true);

            if (string.IsNullOrEmpty(Config.GetToken()) || Config.GetToken().EqualsIgnoreCase("token here")) return;
            await Client.LoginAsync(TokenType.Bot, Config.GetToken());
            await Client.StartAsync();
            if (Config.GetStreamer().EqualsIgnoreCase("streamer here") ||
                string.IsNullOrWhiteSpace(Config.GetStreamer()))
                await Client.SetGameAsync(Config.GetGame());
            else
                await Client.SetGameAsync(Config.GetGame(),
                    $"https://twitch.tv/{Config.GetStreamer()}",
                    ActivityType.Streaming);

            await Client.SetStatusAsync(UserStatus.Online);
            await _handler.InitAsync();
            await Task.Delay(-1);
        }
    }
}