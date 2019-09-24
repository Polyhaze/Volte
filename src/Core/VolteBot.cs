using System;
using System.Diagnostics;
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

        private IServiceProvider _provider;
        private DiscordShardedClient _client;
        private CancellationTokenSource _cts;

        private static void BuildServiceProvider(int shardCount, out IServiceProvider provider)
            => provider = new ServiceCollection() 
                .AddAllServices(shardCount)
                .BuildServiceProvider();

        private VolteBot() 
            => Console.CancelKeyPress += (s, _) => _cts.Cancel();

        private async Task LoginAsync()
        {
            
            Console.Title = "Volte";
            Console.CursorVisible = false;

            if (!Directory.Exists(Config.DataDirectory))
            {
                Console.WriteLine($"The \"{Config.DataDirectory}\" directory didn't exist, so I created it for you.");
                Directory.CreateDirectory(Config.DataDirectory);
                //99.9999999999% of the time the config also won't exist if this block is reached
                //if the config does exist when this block is reached, feel free to become the lead developer of this project
            }

            if (!Config.CreateIfNotExists())
            {
                Console.WriteLine($"Please fill in the configuration located at \"{Config.ConfigFile}\"; restart me when you've done so.");
                return;
            }

            Config.Load();

            if (!Config.IsValidToken()) return;
            int shardCount;
            using (var rest = new DiscordRestClient())
            {
                await rest.LoginAsync(TokenType.Bot, Config.Token);
                shardCount = await rest.GetRecommendedShardCountAsync();
                await rest.LogoutAsync();
            }

            BuildServiceProvider(shardCount, out _provider);

            _provider.Get(out _client);
            _provider.Get(out _cts);
            _provider.Get<VolteHandler>(out var handler);
            _provider.Get<LoggingService>(out var logger);

            await _client.LoginAsync(TokenType.Bot, Config.Token);
            await _client.StartAsync().ContinueWith(_ => _client.SetStatusAsync(UserStatus.Online));

            await handler.InitializeAsync(_provider);

            try
            {
                await Task.Delay(-1, _cts.Token);
            }
            catch (TaskCanceledException)
            {
                //this exception always occurs when CancellationTokenSource#Cancel() is called; so we put the shutdown logic inside the catch block
                logger.Critical(LogSource.Volte,
                    "Bot shutdown requested by the bot owner; shutting down.");
                await ShutdownAsync(_client, _cts);
            }
        }

        // ReSharper disable SuggestBaseTypeForParameter
        private async Task ShutdownAsync(DiscordShardedClient client, CancellationTokenSource cts)
        {
            if (Config.GuildLogging.EnsureValidConfiguration(client, out var channel))
            {
                await new EmbedBuilder()
                    .WithErrorColor()
                    .WithAuthor(client.GetOwner())
                    .WithDescription(
                        $"Volte {Version.FullVersion} is shutting down at **{DateTimeOffset.UtcNow.FormatFullTime()}, on {DateTimeOffset.UtcNow.FormatDate()}**. I was online for **{Process.GetCurrentProcess().GetUptime()}**!")
                    .SendToAsync(channel);
            }
            
            await client.SetStatusAsync(UserStatus.Invisible);
            await client.LogoutAsync();
            await client.StopAsync();
            Dispose(cts, client, DatabaseService.Database);
            Environment.Exit(0);
        }

        private void Dispose(params IDisposable[] disposables)
        {
            foreach (var disposable in disposables)
                disposable.Dispose();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
        }
    }
}