using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using Volte.Core.Commands;
using Volte.Core.Data.Objects;
using Volte.Core.Extensions;
using Volte.Core.Services;

namespace Volte.Core.Discord
{
    public class VolteHandler
    {
        private static readonly IServiceProvider _services = VolteBot.ServiceProvider;
        private readonly AntilinkService _antilink = _services.GetRequiredService<AntilinkService>();
        private readonly BlacklistService _blacklist = _services.GetRequiredService<BlacklistService>();
        private readonly DiscordSocketClient _client = VolteBot.Client;
        private readonly DatabaseService _db = _services.GetRequiredService<DatabaseService>();
        private readonly PingChecksService _pingchecks = _services.GetRequiredService<PingChecksService>();
        private readonly CommandService _service = _services.GetRequiredService<CommandService>();
        private readonly GuildService _guild = _services.GetRequiredService<GuildService>();
        private readonly WelcomeService _welcome = _services.GetRequiredService<WelcomeService>();
        private readonly AutoroleService _autorole = _services.GetRequiredService<AutoroleService>();
        private readonly EventService _event = _services.GetRequiredService<EventService>();
        private readonly LoggingService _logger = _services.GetRequiredService<LoggingService>();


        public async Task InitAsync()
        {
            var sw = Stopwatch.StartNew();
            var loaded = _service.AddModules(Assembly.GetExecutingAssembly());
            sw.Stop();
            await _logger.Log(LogSeverity.Info, LogSource.Volte,
                $"Loaded {loaded.Count} modules and {loaded.Sum(m => m.Commands.Count)} commands loaded in {sw.ElapsedMilliseconds}ms.");
            //register event listeners
            //_service.CommandExecuted += _event.OnCommand;
            _client.Log += _logger.Log;
            _client.JoinedGuild += _guild.OnJoinAsync;
            _client.LeftGuild += _guild.OnLeaveAsync;
            _client.UserJoined += _welcome.JoinAsync;
            _client.UserLeft += _welcome.LeaveAsync;
            _client.UserJoined += _autorole.ApplyRoleAsync;
            _client.Ready += _event.OnReady;
            _client.MessageReceived += async s =>
            {
                if (s.Author.IsBot) return;
                if (s.Channel is IDMChannel)
                {
                    await s.Channel.SendMessageAsync("Currently, I do not support commands via DM.");
                    return;
                }

                if (!(s is SocketUserMessage msg)) return;

                var ctx = new VolteContext(_client, msg);
                await _blacklist.CheckMessageAsync(ctx);
                await _antilink.CheckMessageAsync(ctx);
                await _pingchecks.CheckMessageAsync(ctx);
                await HandleMessageAsync(ctx);
            };
        }

        private async Task HandleMessageAsync(VolteContext ctx)
        {

            //pass the message-reliant services what they need
            await _blacklist.CheckMessageAsync(ctx);
            await _antilink.CheckMessageAsync(ctx);
            await _pingchecks.CheckMessageAsync(ctx);

            var config = _db.GetConfig(ctx.Guild);
            var prefixes = new List<string> {config.CommandPrefix, $"<@{ctx.Client.CurrentUser.Id}> "};
            if (CommandUtilities.HasAnyPrefix(ctx.Message.Content, prefixes, StringComparison.OrdinalIgnoreCase, out _,
                out var cmd))
            {
                var sw = Stopwatch.StartNew();
                var result = await _service.ExecuteAsync(cmd, ctx, _services);

                if (result is CommandNotFoundResult) return;
                var targetCommand = _service.GetAllCommands().FirstOrDefault(x => x.FullAliases.ContainsIgnoreCase(cmd))
                                    ?? _service.GetAllCommands()
                                        .FirstOrDefault(x => x.FullAliases.ContainsIgnoreCase(cmd.Split(' ')[0]));
                sw.Stop();
                await _services.GetRequiredService<EventService>().OnCommandAsync(targetCommand, result, ctx, sw);

                if (config.DeleteMessageOnCommand) await ctx.Message.DeleteAsync();
            }
        }
    }
}