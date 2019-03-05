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
using Volte.Core.Data;
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
        private readonly DefaultWelcomeService _defaultWelcome = _services.GetRequiredService<DefaultWelcomeService>();
        private readonly ImageWelcomeService _imageWelcome = _services.GetRequiredService<ImageWelcomeService>();
        private readonly AutoroleService _autorole = _services.GetRequiredService<AutoroleService>();
        private readonly EventService _event = _services.GetRequiredService<EventService>();
        private readonly LoggingService _logger = _services.GetRequiredService<LoggingService>();
        private readonly VerificationService _verification = _services.GetRequiredService<VerificationService>();


        public async Task InitAsync()
        {
            var sw = Stopwatch.StartNew();
            var loaded = _service.AddModules(Assembly.GetExecutingAssembly());
            sw.Stop();
            await _logger.Log(LogSeverity.Info, LogSource.Volte,
                $"Loaded {loaded.Count} modules and {loaded.Sum(m => m.Commands.Count)} commands loaded in {sw.ElapsedMilliseconds}ms.");
            //register event listeners
            //_service.CommandExecuted += _event.OnCommandAsync;
            _client.Log += _logger.Log;
            _client.JoinedGuild += _guild.OnJoinAsync;
            _client.LeftGuild += _guild.OnLeaveAsync;
            _client.ReactionAdded += _verification.CheckReactionAsync;
            _client.UserJoined += async user =>
            {
                if (Config.WelcomeApiKey.IsNullOrWhitespace())
                    await _defaultWelcome.JoinAsync(user);
                else
                    await _imageWelcome.JoinAsync(user);
            };
            _client.UserLeft += _defaultWelcome.LeaveAsync;
            _client.UserJoined += _autorole.ApplyRoleAsync;
            _client.Ready += _event.OnReady;
            _client.MessageReceived += async s =>
            {
                if (!(s is SocketUserMessage msg)) return;
                if (msg.Author.IsBot) return;
                if (msg.Channel is IDMChannel)
                {
                    await msg.Channel.SendMessageAsync("Currently, I do not support commands via DM.");
                    return;
                }

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
            var prefixes = new [] {config.CommandPrefix, $"<@{ctx.Client.CurrentUser.Id}> "};
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
                await _event.OnCommandAsync(targetCommand, result, ctx, sw);

                if (config.DeleteMessageOnCommand) await ctx.Message.DeleteAsync();
            }
        }
    }
}