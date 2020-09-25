using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using Gommon;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Volte.Core.Helpers;
using Volte.Core.Entities;
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

        private static IServiceProvider BuildServiceProvider()
            => new ServiceCollection() 
                .AddAllServices()
                .BuildServiceProvider();

        private VolteBot() 
            => Console.CancelKeyPress += (_, __) => _cts.Cancel();

        private async Task LoginAsync()
        {
            if (!Config.Initialize()) return;

            _provider = BuildServiceProvider();
            
            _client = _provider.Get<DiscordShardedClient>();
            _cts = _provider.Get<CancellationTokenSource>();
            var logger = _provider.Get<LoggingService>();
            
            await _client.StartAsync();

            Initialize(_provider);

            _ = await _client.UseInteractivityAsync(new InteractivityConfiguration
            {
                PollBehaviour = PollBehaviour.DeleteEmojis,
                PaginationBehaviour = PaginationBehaviour.WrapAround,
                PaginationDeletion = PaginationDeletion.DeleteEmojis,
                PaginationEmojis = new PaginationEmojis
                {
                    Left = EmojiHelper.Back.ToEmoji(),
                    Right = EmojiHelper.Next.ToEmoji(),
                    SkipLeft = EmojiHelper.First.ToEmoji(),
                    SkipRight = EmojiHelper.Last.ToEmoji(),
                    Stop = EmojiHelper.Stop.ToEmoji()
                },
                Timeout = TimeSpan.FromSeconds(15)
            });

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
        public static async Task ShutdownAsync([NotNull] DiscordShardedClient client, [NotNull] IServiceProvider provider)
        {
            if (Config.GuildLogging.EnsureValidConfiguration(client, out var channel))
            {
                await new DiscordEmbedBuilder()
                    .WithErrorColor()
                    .WithDescription(
                        $"Volte {Version.FullVersion} is shutting down at **{DateTimeOffset.UtcNow.FormatFullTime()}, on {DateTimeOffset.UtcNow.FormatDate()}**. I was online for **{Process.GetCurrentProcess().CalculateUptime()}**!")
                    .SendToAsync(channel);
            }
            
            foreach (var disposable in provider.GetServices<IDisposable>())
            {
                disposable?.Dispose();
            }
            
            await client.UpdateStatusAsync(userStatus: UserStatus.Invisible);
            foreach (var (_, shard) in client.ShardClients)
            {
                await shard.DisconnectAsync();
                shard.Dispose();
            }
            Environment.Exit(0);
        }
        
        public void Initialize(IServiceProvider provider)
        {
            var commandService = provider.Get<CommandService>();
            var logger = provider.Get<LoggingService>();
            
            var sw = Stopwatch.StartNew();
            var l = commandService.AddTypeParsersAsync();
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