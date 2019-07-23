using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Data.Models;
using Gommon;
using Microsoft.Extensions.DependencyInjection;
using Volte.Services;

namespace Volte.Core
{
    internal sealed class VolteHandler
    {
        private readonly DiscordShardedClient _client;
        private readonly CommandService _service;
        private readonly LoggingService _logger;

        public VolteHandler(DiscordShardedClient client,
            CommandService commandService,
            LoggingService loggingService)
        {
            _client = client;
            _service = commandService;
            _logger = loggingService;
        }

        public async Task InitAsync(ServiceProvider provider)
        {
            await _service.AddTypeParsersAsync();
            var sw = Stopwatch.StartNew();
            var loaded = _service.AddModules(Assembly.GetExecutingAssembly());
            sw.Stop();
            await _logger.LogAsync(LogSeverity.Info, LogSource.Volte,
                $"Loaded {loaded.Count} modules and {loaded.Sum(m => m.Commands.Count)} commands loaded in {sw.ElapsedMilliseconds}ms.");
            await _client.RegisterVolteEventHandlersAsync(provider);
        }
    }
}