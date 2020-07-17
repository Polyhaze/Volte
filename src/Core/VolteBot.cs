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
using Color = System.Drawing.Color;
using Console = Colorful.Console;

namespace Volte.Core
{
    public class VolteBot
    {
        public static Task StartAsync()
        {
            Console.Title = "Volte";
            Console.CursorVisible = false;
            return new VolteBot().LoginAsync();
        }

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
            if (!Config.StartupChecks()) return; 

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
            _provider.Get<HandlerService>(out var handler);
            _provider.Get<LoggingService>(out var logger);

            await _client.LoginAsync(TokenType.Bot, Config.Token);
            await _client.StartAsync().ContinueWith(_ => _client.SetStatusAsync(UserStatus.Online));

            await handler.InitializeAsync(_provider);

            try
            {
                await Task.Delay(-1, _cts.Token);
            }
            catch (TaskCanceledException) //this exception always occurs when CancellationTokenSource#Cancel() is called; so we put the shutdown logic inside the catch block
            {
                logger.Critical(LogSource.Volte,
                    "Bot shutdown requested; shutting down and cleaning up.");
                await ShutdownAsync(_client);
            }
        }

        // ReSharper disable SuggestBaseTypeForParameter
        public static async Task ShutdownAsync(DiscordShardedClient client)
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
            Environment.Exit(0);
        }
    }
}