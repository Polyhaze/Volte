using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Discord;
using Discord.WebSocket;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Qmmands;
using Sentry;
using Volte;
using Volte.Core;
using Volte.Services;
using Version = Volte.Version;

namespace Gommon
{
    public static partial class Extensions
    {
        public static IServiceCollection AddAllServices(this IServiceCollection coll, int shardCount) =>
            coll.AddVolteServices()
                .AddSingleton(SentrySdk.Init(so =>
                {
                    so.Dsn = Config.SentryDsn;
                    so.Debug = Config.EnableDebugLogging || Version.ReleaseType is Version.DevelopmentStage.Development;
                }))
                .AddSingleton(new HttpClient
                {
                    Timeout = 10.Seconds()
                })
                .AddSingleton<CancellationTokenSource>()
                .AddSingleton(new CommandService(new CommandServiceConfiguration
                {
                    IgnoresExtraArguments = true,
                    StringComparison = StringComparison.OrdinalIgnoreCase,
                    DefaultRunMode = RunMode.Sequential,
                    SeparatorRequirement = SeparatorRequirement.SeparatorOrWhitespace,
                    Separator = " ",
                    NullableNouns = null
                }))
                .AddSingleton(new DiscordShardedClient(new DiscordSocketConfig
                {
                    LogLevel = Version.ReleaseType is Version.DevelopmentStage.Development
                        ? LogSeverity.Debug
                        : LogSeverity.Verbose,
                    GatewayIntents = Intents,
                    AlwaysDownloadUsers = true,
                    ConnectionTimeout = 10000,
                    MessageCacheSize = 50,
                    TotalShards = shardCount
                }));

        private static GatewayIntents Intents
            => GatewayIntents.Guilds | GatewayIntents.GuildMessageReactions | GatewayIntents.GuildMembers |
               GatewayIntents.GuildMessages | GatewayIntents.GuildPresences;

        public static IServiceCollection AddVolteServices(this IServiceCollection serviceCollection)
            => serviceCollection.Apply(coll =>
            {
                //get all the classes that inherit VolteService, and aren't abstract.
                foreach (var service in typeof(Program).Assembly.GetTypes()
                    .Where(t => t.Inherits<VolteService>() && !t.IsAbstract))
                {
                    coll.TryAddSingleton(service);
                }
            });

        public static T Get<T>(this IServiceProvider provider)
            => provider.GetRequiredService<T>();

        public static void Get<T>(this IServiceProvider provider, out T service)
            => service = provider.GetRequiredService<T>();
    }
}