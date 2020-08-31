using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Qmmands;
using RestSharp;
using Volte;
using Volte.Core;
using Volte.Services;
using Version = Volte.Version;

namespace Gommon
{
    public static partial class Extensions
    {
        public static IServiceCollection AddAllServices(this IServiceCollection coll)
        {
            var logger = new LoggingService();
            //add all other services; formerly in the VolteBot class
            coll.AddVolteServices()
                .AddSingleton(new RestClient {UserAgent = $"Volte/{Version.FullVersion}"})
                .AddSingleton<HttpClient>()
                .AddSingleton<CancellationTokenSource>()
                .AddSingleton(logger)
                .AddSingleton(new CommandService(new CommandServiceConfiguration
                {
                    IgnoresExtraArguments = true,
                    Separator = " "
                }))
                .AddSingleton(new DiscordShardedClient(new DiscordConfiguration
                {
                    LoggerFactory = logger,
                    HttpTimeout = TimeSpan.FromSeconds(10),
                    MessageCacheSize = 50,
                    TokenType = TokenType.Bot,
                    Token = Config.Token
                }));
            return coll;
        }

        public static IServiceCollection AddVolteServices(this IServiceCollection coll)
        {
            //get all the classes that are Volte[Event]Services, aren't abstract, and don't have the System.ObsoleteAttribute attribute.
            foreach (var service in typeof(Program).Assembly.GetTypes()
                .Where(t => !t.HasAttribute<ObsoleteAttribute>() && (
                            t.Inherits<VolteEventService>() 
                            || t.Inherits<VolteService>()) && !t.IsAbstract))
            {
                coll.TryAddSingleton(service);
            }

            return coll;
        }

        public static T Get<T>(this IServiceProvider provider)
            => provider.GetRequiredService<T>();

        public static void Get<T>(this IServiceProvider provider, out T service)
            => service = provider.GetRequiredService<T>();
    }
}