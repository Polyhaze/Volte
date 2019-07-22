using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Data;
using Volte.Data.Models;
using Volte.Data.Models.EventArgs;
using Gommon;
using Microsoft.Extensions.DependencyInjection;
using Volte.Services;

namespace Volte.Core
{
    internal sealed class VolteHandler
    {
        private readonly DiscordShardedClient _client;
        private readonly CommandService _service;
        private readonly GuildService _guild;
        private readonly WelcomeService _welcome;
        private readonly AutoroleService _autorole;
        private readonly EventService _event;
        private readonly LoggingService _logger;

        public VolteHandler(DiscordShardedClient client,
            CommandService commandService,
            GuildService guildService,
            WelcomeService welcomeService,
            AutoroleService autoroleService,
            EventService eventService,
            LoggingService loggingService)
        {
            _client = client;
            _service = commandService;
            _guild = guildService;
            _welcome = welcomeService;
            _autorole = autoroleService;
            _event = eventService;
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
            _client.Log += m => _logger.Log(new LogEventArgs(m));
            _client.JoinedGuild += guild => _guild.OnJoinAsync(new JoinedGuildEventArgs(guild));
            _client.LeftGuild += guild => _guild.OnLeaveAsync(new LeftGuildEventArgs(guild));
            _client.UserJoined += user =>
            {
                if (Config.EnabledFeatures.Welcome)
                    return _welcome.JoinAsync(new UserJoinedEventArgs(user));
                if (Config.EnabledFeatures.Autorole)
                    return _autorole.ApplyRoleAsync(new UserJoinedEventArgs(user));
                return Task.CompletedTask;
            };
            _client.UserLeft += user =>
            {
                if (Config.EnabledFeatures.Welcome)
                    return _welcome.LeaveAsync(new UserLeftEventArgs(user));
                return Task.CompletedTask;
            };
            _client.ShardReady += (client) => _event.OnReady(new ReadyEventArgs(client));
            _client.MessageReceived += async (s) =>
            {
                if (!(s is IUserMessage msg)) return;
                if (msg.Author.IsBot) return;
                if (msg.Channel is IDMChannel)
                {
                    await msg.Channel.SendMessageAsync("Currently, I do not support commands via DM.");
                    return;
                }

                await _event.HandleMessageAsync(new MessageReceivedEventArgs(s, provider));
            };
        }
    }
}