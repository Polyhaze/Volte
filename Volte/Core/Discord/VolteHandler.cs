using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Services;
using Volte.Core.Data;
using Volte.Core.Extensions;
using Volte.Core.Commands;

namespace Volte.Core.Discord {
    public class VolteHandler {
        private readonly DiscordSocketClient _client = VolteBot.Client;
        private readonly CommandService _service = VolteBot.CommandService;
        private readonly BlacklistService _blacklist = _services.GetRequiredService<BlacklistService>();
        private readonly AntilinkService _antilink = _services.GetRequiredService<AntilinkService>();
        private readonly EconomyService _economy = _services.GetRequiredService<EconomyService>();
        private readonly PingChecksService _pingchecks = _services.GetRequiredService<PingChecksService>();

        private static readonly IServiceProvider _services = VolteBot.ServiceProvider;

        public async Task Init() {
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            //register event listeners
            _service.CommandExecuted += _services.GetRequiredService<EventService>().OnCommand;
            _client.MessageReceived += HandleMessageOrCommand;
            _client.JoinedGuild += _services.GetRequiredService<GuildService>().OnJoin;
            _client.LeftGuild += _services.GetRequiredService<GuildService>().OnLeave;
            _client.UserJoined += _services.GetRequiredService<WelcomeService>().Join;
            _client.UserJoined += _services.GetRequiredService<AutoroleService>().Apply;
            _client.UserLeft += _services.GetRequiredService<WelcomeService>().Leave;
            _client.Ready += _services.GetRequiredService<EventService>().OnReady;
        }

        private async Task HandleMessageOrCommand(SocketMessage s) {
            var argPos = 0; //i'd get rid of this but because Discord.Net requires a ref param i can't.
            var msg = (SocketUserMessage) s;
            var ctx = new VolteContext(_client, msg);
            if (ctx.User.IsBot) return;

            //pass the message-reliant services what they need
            await _blacklist.CheckMessage(ctx);
            await _antilink.CheckMessage(ctx);
            await _economy.Give(ctx);
            await _pingchecks.CheckMessage(ctx);

            var config = _services.GetRequiredService<DatabaseService>().GetConfig(ctx.Guild);
            _services.GetRequiredService<DatabaseService>().GetUser(s.Author.Id);
            var prefix = string.IsNullOrEmpty(config.CommandPrefix) ? Config.GetCommandPrefix() : config.CommandPrefix;
            var msgStrip = msg.Content.Replace(prefix, string.Empty);
            if (msg.HasStringPrefix(prefix, ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos)) {
                var result = await _service.ExecuteAsync(ctx, argPos, _services);

                if (result.ErrorReason.Equals("Unknown command.")) return;

                if (config.DeleteMessageOnCommand) {
                    await ctx.Message.DeleteAsync();
                }
            }
        }
    }
}