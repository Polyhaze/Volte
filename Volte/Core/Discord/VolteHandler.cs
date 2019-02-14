using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Services;
using Volte.Core.Commands;
using Volte.Core.Extensions;

namespace Volte.Core.Discord {
    public class VolteHandler {
        private readonly DiscordSocketClient _client = VolteBot.Client;
        private readonly CommandService _service = VolteBot.CommandService;
        private readonly BlacklistService _blacklist = _services.GetRequiredService<BlacklistService>();
        private readonly AntilinkService _antilink = _services.GetRequiredService<AntilinkService>();
        private readonly EconomyService _economy = _services.GetRequiredService<EconomyService>();
        private readonly PingChecksService _pingchecks = _services.GetRequiredService<PingChecksService>();
        private readonly DatabaseService _db = _services.GetRequiredService<DatabaseService>();

        private static readonly IServiceProvider _services = VolteBot.ServiceProvider;

        public Task Init() {
            _service.AddModules(Assembly.GetEntryAssembly());
            //register event listeners
            //_service.CommandExecuted += _services.GetRequiredService<EventService>().OnCommand;
            _client.MessageReceived += HandleMessageOrCommand;
            _client.JoinedGuild += _services.GetRequiredService<GuildService>().OnJoin;
            _client.LeftGuild += _services.GetRequiredService<GuildService>().OnLeave;
            _client.UserJoined += _services.GetRequiredService<WelcomeService>().Join;
            _client.UserJoined += _services.GetRequiredService<AutoroleService>().Apply;
            _client.UserLeft += _services.GetRequiredService<WelcomeService>().Leave;
            _client.Ready += _services.GetRequiredService<EventService>().OnReady;
            return Task.CompletedTask;
        }

        private async Task HandleMessageOrCommand(SocketMessage s) {
            if (!(s is SocketUserMessage msg)) return;
            var ctx = new VolteContext(_client, msg);
            if (ctx.User.IsBot) return;

            //pass the message-reliant services what they need
            await _blacklist.CheckMessage(ctx);
            await _antilink.CheckMessage(ctx);
            await _economy.Give(ctx);
            await _pingchecks.CheckMessage(ctx);

            var config = _db.GetConfig(ctx.Guild);
            IEnumerable<string> prefixes = new List<string> {config.CommandPrefix, $"{ctx.Client.CurrentUser.Mention}"};
            _db.GetUser(s.Author.Id);
            if (CommandUtilities.HasAnyPrefix(msg.Content, prefixes, StringComparison.OrdinalIgnoreCase, out _, out var cmd)) {
                if (ctx.Channel is IDMChannel) {
                    await ctx.ReactFailure();
                    return;
                }
                var result = await _service.ExecuteAsync(cmd, ctx, _services);

                if (result is CommandNotFoundResult) return;
                var targetCommand = _service.GetAllCommands().FirstOrDefault(x => x.FullAliases.ContainsIgnoreCase(cmd)) 
                                    ?? _service.GetAllCommands()
                                        .FirstOrDefault(x => x.FullAliases.ContainsIgnoreCase(cmd.Split(' ')[0]));
                await _services.GetRequiredService<EventService>().OnCommand(targetCommand, result, ctx, _services);

                if (config.DeleteMessageOnCommand) {
                    await ctx.Message.DeleteAsync();
                }
            }
        }
    }
}