using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SIVA.Core.Discord.Automation;
using SIVA.Core.Files.Readers;
using SIVA.Core.Runtime;

namespace SIVA.Core.Discord {
    public class SIVA {
        public static IServiceProvider ServiceProvider = BuildServiceProvider();
        public static Log GetLogger() => Runtime.Log.GetLogger();
        public static readonly DiscordSocketClient Client = 
            new DiscordSocketClient(new DiscordSocketConfig {LogLevel = LogSeverity.Verbose});
        public static readonly SIVAHandler Handler = new SIVAHandler();
        public static readonly Log Logger = new Log();
        
        /// <summary>
        ///     WARNING:
        ///     Instantiating this object will start a completely new bot instance.
        ///     Don't do that, unless you're making a restart function!
        /// </summary>
        public SIVA() {
            GetLogger().PrintVersion();
            LoginAsync().GetAwaiter().GetResult();
        }
        
        private static IServiceProvider BuildServiceProvider() {
            var commandServiceConfig = new CommandServiceConfig {
                IgnoreExtraArgs = true,
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false,
                LogLevel = LogSeverity.Verbose
            };
            return new ServiceCollection()
                .AddSingleton<AntilinkService>()
                .AddSingleton<AutoroleService>()
                .AddSingleton<BlacklistService>()
                .AddSingleton<EconomyService>()
                .AddSingleton<WelcomeService>()
                .AddSingleton(Handler)
                .AddSingleton(new CommandService(commandServiceConfig))
                .AddSingleton(Client)
                .BuildServiceProvider();
        }
        
        public static async Task LoginAsync() {
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