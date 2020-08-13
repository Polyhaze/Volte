using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Gommon;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Volte.Core.Models;
using Volte.Services;
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

        private static IServiceProvider BuildServiceProvider(int shardCount)
            => new ServiceCollection() 
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
            
            _provider = BuildServiceProvider(shardCount);
            
            _client = _provider.Get<DiscordShardedClient>();
            _cts = _provider.Get<CancellationTokenSource>();
            var logger = _provider.Get<LoggingService>();

            await _client.LoginAsync(TokenType.Bot, Config.Token);
            await _client.StartAsync().ContinueWith(_ => _client.SetStatusAsync(UserStatus.Online));

            await InitializeAsync(_provider);

            try
            {
                await Task.Delay(-1, _cts.Token);
            }
            catch (TaskCanceledException) //this exception always occurs when CancellationTokenSource#Cancel() is called; so we put the shutdown logic inside the catch block
            {
                logger.Critical(LogSource.Volte,
                    "Bot shutdown requested; shutting down and cleaning up.");
                await ShutdownAsync(_client, _provider);
            }
        }

        // ReSharper disable SuggestBaseTypeForParameter
        public static async Task ShutdownAsync(DiscordShardedClient client, IServiceProvider provider)
        {
            if (Config.GuildLogging.EnsureValidConfiguration(client, out var channel))
            {
                await new EmbedBuilder()
                    .WithErrorColor()
                    .WithAuthor(client.GetOwner())
                    .WithDescription(
                        $"Volte {Version.FullVersion} is shutting down at **{DateTimeOffset.UtcNow.FormatFullTime()}, on {DateTimeOffset.UtcNow.FormatDate()}**. I was online for **{Process.GetCurrentProcess().CalculateUptime()}**!")
                    .SendToAsync(channel);
            }
            
            foreach (var disposable in provider.GetServices<IDisposable>())
            {
                disposable?.Dispose();
            }
            
            await client.SetStatusAsync(UserStatus.Invisible);
            await client.LogoutAsync();
            await client.StopAsync();
            Environment.Exit(0);
        }
        
        public async Task InitializeAsync(IServiceProvider provider)
        {
            var commandService = provider.Get<CommandService>();
            var logger = provider.Get<LoggingService>();
            
            var sw = Stopwatch.StartNew();
            var l = await commandService.AddTypeParsersAsync();
            sw.Stop();
            logger.Info(LogSource.Volte, $"Loaded TypeParsers: [{l.Select(x => x.SanitizeParserName()).Join(", ")}] in {sw.ElapsedMilliseconds}ms.");
            sw = Stopwatch.StartNew();
            var loaded = commandService.AddModules(GetType().Assembly, null, module =>
                {
                    module.WithRunMode(RunMode.Sequential);
                });
            sw.Stop();
            logger.Info(LogSource.Volte,
                $"Loaded {loaded.Count} modules and {loaded.Sum(m => m.Commands.Count)} commands in {sw.ElapsedMilliseconds}ms.");
            _client.RegisterVolteEventHandlers(provider);
        }
    }
}