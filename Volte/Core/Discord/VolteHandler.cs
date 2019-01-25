using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Services;
using Volte.Core.Files.Readers;
using Volte.Core.Modules;
using Volte.Core.Runtime;

namespace Volte.Core.Discord {
    public class VolteHandler {
        private readonly DiscordSocketClient _client = VolteBot.Client;
        private readonly CommandService _service = VolteBot.CommandService;

        private readonly IServiceProvider _services = VolteBot.ServiceProvider;

        public async Task Init() {
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            _service.CommandExecuted += _services.GetRequiredService<EventService>().OnCommand;
            _client.MessageReceived += HandleMessageOrCommand;
            _client.JoinedGuild += _services.GetRequiredService<EventService>().Guilds;
            _client.UserJoined += _services.GetRequiredService<WelcomeService>().Join;
            _client.UserJoined += _services.GetRequiredService<AutoroleService>().Apply;
            _client.UserLeft += _services.GetRequiredService<WelcomeService>().Leave;
            _client.Ready += _services.GetRequiredService<EventService>().OnReady;
        }

        private async Task HandleMessageOrCommand(SocketMessage s) {
            var argPos = 0; //i'd get rid of this but because Discord.Net requires a ref param i can't.
            var msg = (SocketUserMessage) s;
            var ctx = new VolteContext(_client, msg);
            await _services.GetRequiredService<BlacklistService>().CheckMessage(s);
            await _services.GetRequiredService<AntilinkService>().CheckMessage(s);
            await _services.GetRequiredService<EconomyService>().Give(ctx);
            if (ctx.User.IsBot) return;
            var config = _services.GetRequiredService<DatabaseService>().GetConfig(ctx.Guild);
            _services.GetRequiredService<DatabaseService>().GetUser(s.Author.Id);
            var prefix = config.CommandPrefix == string.Empty ? Config.GetCommandPrefix() : config.CommandPrefix;
            var msgStrip = msg.Content.Replace(prefix, string.Empty);
            if (msg.HasStringPrefix(prefix, ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos)) {
                var result = await _service.ExecuteAsync(ctx, argPos, _services);
                if (!result.IsSuccess && result.ErrorReason != "Unknown command.") {
                    string reason;
                    switch (result.ErrorReason) {
                        case "The server responded with error 403: Forbidden":
                            reason =
                                "I'm not allowed to do that. Either I don't have permission or the requested user is higher than me in the role hierarchy.";
                            break;
                        case "Failed to parse Boolean":
                            reason = "You can only input `true` or `false` for this command.";
                            break;
                        default:
                            reason = result.ErrorReason;
                            break;
                    }

                    var embed = new EmbedBuilder();

                    if (msg.HasMentionPrefix(_client.CurrentUser, ref argPos)) {
                        var nm = msg.Content.Replace($"<@{_client.CurrentUser.Id}> ", config.CommandPrefix);
                        embed.AddField("Error in command:", nm);
                        embed.AddField("Error reason:", reason);
                        embed.AddField("Weird error?",
                            "[Report it in the SIVA-dev server](https://discord.gg/prR9Yjq)");
                        embed.WithAuthor(ctx.User);
                        embed.WithColor(Config.GetErrorColour());
                        await ctx.Channel.SendMessageAsync("", false, embed.Build());
                    }
                    else {
                        var nm = msg.Content;
                        embed.AddField("Error in command:", nm);
                        embed.AddField("Error reason:", reason);
                        embed.AddField("Weird error?",
                            "[Report it in the SIVA-dev server](https://discord.gg/prR9Yjq)");
                        embed.WithAuthor(ctx.User);
                        embed.WithColor(Config.GetErrorColour());
                        await ctx.Channel.SendMessageAsync("", false, embed.Build());
                    }
                }

                if (result.ErrorReason.Equals("Unknown command.")) return;

                if (config.DeleteMessageOnCommand) {
                    await ctx.Message.DeleteAsync();
                }

                if (config.CustomCommands.ContainsKey(msgStrip)) {
                    await ctx.Channel.SendMessageAsync(
                        config.CustomCommands.FirstOrDefault(c => c.Key.ToLower() == msgStrip.ToLower()).Value
                    );
                }
            }
            else {
                if (msg.Content.Contains($"<@{_client.CurrentUser.Id}>")) {
                    await ctx.Channel.SendMessageAsync("<:whO_PENG:437088256291504130>");
                }
            }
        }
    }
}