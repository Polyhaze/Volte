using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Gommon;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Models;
using Volte.Services;

namespace Volte.Core
{
    public class VolteBot
    {
        public static Task StartAsync()
            => new VolteBot().LoginAsync();

        private static void BuildServiceProvider(int shardCount, out IServiceProvider provider)
            => provider = new ServiceCollection() 
                .AddAllServices(shardCount)
                .BuildServiceProvider();

        private VolteBot()
        { }

        private async Task LoginAsync()
        {
            Console.Title = "Volte";
            Console.CursorVisible = false;

            if (!Directory.Exists("data"))
            {
                Console.WriteLine("The \"data\" directory didn't exist, so I created it for you.");
                Directory.CreateDirectory("data");
                //99.9999999999% of the time the config also won't exist if this block is reached
            }

            if (!Config.CreateIfNotExists())
            {
                Console.WriteLine($"Please fill in the configuration located at \"{Config.ConfigFile}\"; restart me when you've done so.");
                return;
            }

            Config.Load();

            if (Config.Token.IsNullOrEmpty() || Config.Token.EqualsIgnoreCase("token here")) return;

            using var rest = new DiscordRestClient();

            await rest.LoginAsync(TokenType.Bot, Config.Token);
            var shardCount = await rest.GetRecommendedShardCountAsync();
            await rest.LogoutAsync();

            BuildServiceProvider(shardCount, out var provider);

            provider.Get<DiscordShardedClient>(out var client);
            provider.Get<CancellationTokenSource>(out var cts);
            provider.Get<VolteHandler>(out var handler);
            provider.Get<LoggingService>(out var logger);

            await client.LoginAsync(TokenType.Bot, Config.Token);
            await client.StartAsync().ContinueWith(_ => client.SetStatusAsync(UserStatus.Online));

            await handler.InitializeAsync(provider);

            try
            {
                await Task.Delay(-1, cts.Token);
            }
            catch (TaskCanceledException)
            {
                //this exception always occurs when CancellationTokenSource#Cancel() is called; so we put the shutdown logic inside the catch block
                logger.Critical(LogSource.Volte,
                    "Bot shutdown requested by the bot owner; shutting down.");
                await ShutdownAsync(client, cts);
            }
        }

        // ReSharper disable SuggestBaseTypeForParameter
        private async Task ShutdownAsync(DiscordShardedClient client, CancellationTokenSource cts)
        {
            await client.SetStatusAsync(UserStatus.Invisible);
            await client.LogoutAsync();
            await client.StopAsync();
            Dispose(cts, client, DatabaseService.Database);
            Environment.Exit(0);
        }

        private void Dispose(params IDisposable[] disposables)
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }
}