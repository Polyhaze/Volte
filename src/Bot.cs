using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Sentry;
using Volte.Commands.Modules;
using Volte.Entities;
using Volte.Helpers;
using Volte.Interactions;
using Volte.Services;
using Console = Colorful.Console;

namespace Volte
{
    public class Bot
    {
        internal static async Task Main(string[] args)
        {
            if (args.ContainsIgnoreCase("--force-command-update")) 
                CommandUpdatingService.ForceUpdateAllCommands = true;

            await StartAsync();
        }
        
        public static Task StartAsync()
        {
            Console.Title = $"Volte V{Version.FullVersion}";
            Console.CursorVisible = false;
            return new Bot().LoginAsync();
        }

        private IServiceProvider _provider;
        private DiscordShardedClient _client;
        private CancellationTokenSource _cts;

        private static IServiceProvider BuildServiceProvider(int shardCount)
            => new ServiceCollection()
                .AddAllServices(shardCount)
                .BuildServiceProvider();

        private Bot()
            => Console.CancelKeyPress += (_, __) => _cts?.Cancel();

        private async Task LoginAsync()
        {
            if (!Config.StartupChecks()) return;

            Config.Load();

            if (!Config.IsValidToken()) return;

            _provider = BuildServiceProvider(await DiscordHelper.GetRecommendedShardCountAsync());
            _client = _provider.Get<DiscordShardedClient>();
            _cts = _provider.Get<CancellationTokenSource>();

            AdminUtilityModule.AllowedPasteSites = await HttpHelper.GetAllowedPasteSitesAsync(_provider);

            await _client.LoginAsync(TokenType.Bot, Config.Token);
            await _client.StartAsync();

            var commandService = _provider.Get<CommandService>();
            
            var l = commandService.AddTypeParsers();
            Logger.Info(LogSource.Volte,
                $"Loaded TypeParsers: [{l.Select(x => x.Name.Replace("Parser", string.Empty)).Join(", ")}]");
            var sw = Stopwatch.StartNew();
            var loaded = commandService.AddModules(GetType().Assembly);
            sw.Stop();
            Logger.Info(LogSource.Volte,
                $"Loaded {loaded.Count} modules and {loaded.Sum(m => m.Commands.Count)} commands in {sw.ElapsedMilliseconds}ms.");
            _client.RegisterCoreEventHandlers(_provider);

            // initializing addons is extremely long-running (because each addon is evaluated sequentially), so don't await
            Executor.Execute(async () => await _provider.Get<AddonService>().InitAsync());
            _provider.Get<ReminderService>().Initialize();
            await _provider.Get<InteractionService>().CommandUpdater.InitAsync();

            try
            {
                await Task.Delay(-1, _cts.Token);
            }
            catch
            {
                try
                {
                    await ShutdownAsync(_client, _provider);
                }
                catch
                {
                    // ignored
                }
            }
        }
        
        public static async Task ShutdownAsync(DiscordShardedClient client, IServiceProvider provider)
        {
            Logger.Critical(LogSource.Volte,
                "Bot shutdown requested; shutting down and cleaning up.");

            if (Config.GuildLogging.TryValidate(client, out var channel))
            {
                await new EmbedBuilder()
                    .WithErrorColor()
                    .WithAuthor(Lambda.TryOrNull(client.GetOwner))
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