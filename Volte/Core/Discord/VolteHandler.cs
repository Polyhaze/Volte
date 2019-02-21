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


        public Task Init()
        {
            _service.AddModules(Assembly.GetEntryAssembly());
            //register event listeners
            //_service.CommandExecuted += _services.GetRequiredService<EventService>().OnCommand;
            _client.MessageReceived += HandleMessageOrCommand;
            _client.JoinedGuild += _guild.OnJoinAsync;
            _client.LeftGuild += _guild.OnLeaveAsync;
            _client.UserJoined += _welcome.JoinAsync;
            _client.UserLeft += _welcome.LeaveAsync;
            _client.UserJoined += _autorole.ApplyRoleAsync;
            _client.Ready += _event.OnReady;
            return Task.CompletedTask;
        }

        private async Task HandleMessageOrCommand(SocketMessage s)
        {
            if (s.Author.IsBot) return;
            if (s.Channel is IDMChannel)
            {
                await s.Channel.SendMessageAsync("Currently, I do not support commands via DM.");
                return;
            }
            if (!(s is SocketUserMessage msg)) return;
            var ctx = new VolteContext(_client, msg);

            //pass the message-reliant services what they need
            await _blacklist.CheckMessageAsync(ctx);
            await _antilink.CheckMessageAsync(ctx);
            await _pingchecks.CheckMessageAsync(ctx);

            var config = _db.GetConfig(ctx.Guild);
            IEnumerable<string> prefixes = new List<string> {config.CommandPrefix, $"<@{ctx.Client.CurrentUser.Id}> "};
            _db.GetUser(s.Author.Id);
            if (CommandUtilities.HasAnyPrefix(msg.Content, prefixes, StringComparison.OrdinalIgnoreCase, out _,
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