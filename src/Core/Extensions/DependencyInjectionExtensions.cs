using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using Discord;
using Discord.WebSocket;
using Volte;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using RestSharp;
using Volte.Core;
using Volte.Services;
using Version = Volte.Version;

namespace Gommon
{
    public static partial class Extensions
    {
        public static IServiceCollection AddAllServices(this IServiceCollection coll, int shardCount) =>
            //add all other services; formerly in the VolteBot class
            coll.AddVolteServices()
                .AddSingleton<VolteHandler>()
                .AddSingleton(new RestClient {UserAgent = $"Volte/{Version.FullVersion}"})
                .AddSingleton(new HttpClient())
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
                .AddSingleton(new DiscordShardedClient(new DiscordSocketConfig
                {
                    LogLevel = Version.ReleaseType is ReleaseType.Release
                        ? LogSeverity.Verbose
                        : LogSeverity.Debug,
                    AlwaysDownloadUsers = true,
                    ConnectionTimeout = 10000,
                    MessageCacheSize = 50,
                    TotalShards = shardCount
                }));

        public static IServiceCollection AddVolteServices(this IServiceCollection provider)
        {
            //get all the classes that have the ServiceAttribute attribute and don't have the System.ObsoleteAttribute attribute.
            foreach (var service in Assembly.GetEntryAssembly()?.GetTypes()?
                .Where(t => !t.HasAttribute<ObsoleteAttribute>() &&
                            t.HasAttribute<ServiceAttribute>()))
            {
                provider.AddSingleton(service);
            }

            return provider;
        }
    }
}