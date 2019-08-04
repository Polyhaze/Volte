﻿using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
                .AddSingleton<HttpClient>()
                .AddSingleton<CancellationTokenSource>()
                .AddSingleton(new CommandService(new CommandServiceConfiguration
                {
                    IgnoreExtraArguments = true,
                    CaseSensitive = false,
                    DefaultRunMode = RunMode.Sequential,
                    SeparatorRequirement = SeparatorRequirement.None,
                    Separator = "irrelevant",
                    NullableNouns = null
                }))
                .AddSingleton(new DiscordShardedClient(new DiscordSocketConfig
                {
                    LogLevel = Config.EnableDebugLogging
                        ? LogSeverity.Debug
                        : LogSeverity.Verbose,
                    AlwaysDownloadUsers = true,
                    ConnectionTimeout = 10000,
                    MessageCacheSize = 50,
                    TotalShards = shardCount
                }));

        public static IServiceCollection AddVolteServices(this IServiceCollection coll)
        {
            //get all the classes that are Volte[Event]Services, aren't abstract, and don't have the System.ObsoleteAttribute attribute.
            foreach (var service in Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => !t.HasAttribute<ObsoleteAttribute>() && (
                            typeof(VolteEventService).IsAssignableFrom(t) 
                            || typeof(VolteService).IsAssignableFrom(t)) && !t.IsAbstract))
            {
                coll.TryAddSingleton(service);
            }

            return coll;
        }

        public static void Get<T>(this ServiceProvider provider, out T service) 
            => service = provider.GetRequiredService<T>();
    }
}