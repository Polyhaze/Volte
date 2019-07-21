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
        public static IServiceCollection AddVolteServices(this IServiceCollection provider)
        {
            //get all the classes that implement the IService interface, arent an interface themselves, and haven't been marked as Obsolete like the GitHubService.
            foreach (var service in Assembly.GetEntryAssembly().GetTypes()
                .Where(t => !t.HasAttribute<ObsoleteAttribute>() &&
                            t.HasAttribute<ServiceAttribute>()))
            {
                provider.AddSingleton(service);
            }

            provider.AddSingleton<VolteHandler>()
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
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = Version.ReleaseType is ReleaseType.Release
                        ? LogSeverity.Verbose
                        : LogSeverity.Debug,
                    AlwaysDownloadUsers = true,
                    ConnectionTimeout = 10000,
                    MessageCacheSize = 50
                }));

            return provider;
        }
    }
}