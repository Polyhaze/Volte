using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Models;
using Gommon;

namespace Volte.Services
{
    internal sealed class HandlerService
    {
        private readonly DiscordShardedClient _client;
        private readonly CommandService _service;
        private readonly LoggingService _logger;

        public HandlerService(DiscordShardedClient client,
            CommandService commandService,
            LoggingService loggingService)
        {
            _client = client;
            _service = commandService;
            _logger = loggingService;
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            var sw = Stopwatch.StartNew();
            var l = await _service.AddTypeParsersAsync();
            sw.Stop();
            _logger.Info(LogSource.Volte, $"Loaded TypeParsers: [{l.Select(x => x.SanitizeParserName()).Join(", ")}] in {sw.ElapsedMilliseconds}ms.");
            sw = Stopwatch.StartNew();

            var loaded = _service.AddModules(GetType().Assembly);
            sw.Stop();
            _logger.Info(LogSource.Volte,
                $"Loaded {loaded.Count} modules and {loaded.Sum(m => m.Commands.Count)} commands loaded in {sw.ElapsedMilliseconds}ms.");
            await _client.RegisterVolteEventHandlersAsync(provider);
        }
    }
}