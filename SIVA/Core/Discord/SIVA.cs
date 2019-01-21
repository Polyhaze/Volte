using System;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SIVA.Core.Discord.Automation;
using SIVA.Core.Runtime;

namespace SIVA.Core.Discord {
    public class SIVA {
        public static IServiceProvider ServiceProvider = BuildServiceProvider();
        public static Log GetLogger() => Log.GetLogger();
        public static SIVAHandler GetEventHandler() => DiscordLogin.Handler;
        public static DiscordSocketClient GetInstance() => DiscordLogin.Client;
        
        /// <summary>
        ///     WARNING:
        ///     Instantiating this object will start a completely new bot instance.
        ///     Don't do that, unless you're making a restart function!
        /// </summary>
        public SIVA() {
            GetLogger().PrintVersion();
            DiscordLogin.LoginAsync().GetAwaiter().GetResult();
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
                .AddSingleton(GetInstance())
                .AddSingleton(new CommandService(commandServiceConfig))
                .AddSingleton(GetEventHandler())
                .BuildServiceProvider();
        }
    }
}