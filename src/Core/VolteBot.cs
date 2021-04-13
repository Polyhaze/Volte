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
using Sentry;
using Volte.Core.Entities;
using Volte.Core.Helpers;
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
            => Console.CancelKeyPress += (_, __) => _cts.Cancel();

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

            await _client.LoginAsync(TokenType.Bot, Config.Token);
            await _client.StartAsync();

            var commandService = _provider.Get<CommandService>();

            var sw = Stopwatch.StartNew();
            var l = commandService.AddTypeParsers();
            sw.Stop();
            Logger.Info(LogSource.Volte, $"Loaded TypeParsers: [{l.Select(x => x.SanitizeParserName()).Join(", ")}] in {sw.ElapsedMilliseconds}ms.");
            sw = Stopwatch.StartNew();
            var loaded = commandService.AddModules(GetType().Assembly);
            sw.Stop();
            Logger.Info(LogSource.Volte,
                $"Loaded {loaded.Count} modules and {loaded.Sum(m => m.Commands.Count)} commands in {sw.ElapsedMilliseconds}ms.");
            _client.RegisterVolteEventHandlers(_provider);

            Executor.Execute(async () => await _provider.Get<AddonService>().InitAsync());
            _provider.Get<ReminderService>().Initialize();

            try
            {
                await Task.Delay(-1, _cts.Token);
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                await ShutdownAsync(_client, _provider);
            }
        }

        // ReSharper disable SuggestBaseTypeForParameter
        public static async Task ShutdownAsync(DiscordShardedClient client, IServiceProvider provider)
        {
            Logger.Critical(LogSource.Volte,
                "Bot shutdown requested; shutting down and cleaning up.");
            
            if (Config.GuildLogging.EnsureValidConfiguration(client, out var channel))
            {
                await new EmbedBuilder()
                    .WithErrorColor()
                    .WithAuthor(client.GetOwner())
                    .WithDescription(
                        $"Volte {Version.FullVersion} is shutting down {DateTime.Now.FormatBoldString()}. I was online for **{Process.GetCurrentProcess().CalculateUptime()}**!")
                    .SendToAsync(channel);
            }
            
            foreach (var disposable in provider.GetServices<IDisposable>())
                disposable?.Dispose();

            await client.SetStatusAsync(UserStatus.Invisible);
            await client.LogoutAsync();
            await client.StopAsync();
            Environment.Exit(0);
        }
    }
}