using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SIVA.Core.Discord.Automation;
using SIVA.Core.Files.Readers;
using SIVA.Core.Runtime;

namespace SIVA.Core.Discord {
    public class SIVAHandler {
        private DiscordSocketClient _client;
        private CommandService _service;
        private Log _logger = SIVA.GetLogger();

        private IServiceProvider BuildServiceProvider() =>
            new ServiceCollection()
                .AddSingleton<AntilinkService>()
                .AddSingleton<AutoroleService>()
                .AddSingleton<BlacklistService>()
                .AddSingleton<EconomyService>()
                .AddSingleton<WelcomeService>()
                .AddSingleton(_client)
                .AddSingleton(_service)
                .AddSingleton(this)
                .BuildServiceProvider();

        public IServiceProvider ServiceProvider => BuildServiceProvider();

        public async Task Init() {
            var config = new CommandServiceConfig {
                IgnoreExtraArgs = true,
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false,
                LogLevel = LogSeverity.Verbose
            };
            _client = SIVA.GetInstance();
            _service = new CommandService(config);
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), BuildServiceProvider());
            _client.MessageReceived += HandleMessageOrCommand;
            _client.JoinedGuild += Guilds;
            _client.UserJoined += ServiceProvider.GetRequiredService<WelcomeService>().Join;
            _client.UserJoined += ServiceProvider.GetRequiredService<AutoroleService>().Apply;
            _client.UserLeft += ServiceProvider.GetRequiredService<WelcomeService>().Leave;
            _client.Ready += OnReady;
        }

        private async Task OnReady() {
            var dbl = SIVA.GetInstance().GetGuild(264445053596991498);
            if (dbl == null || Config.GetOwner() == 168548441939509248) return;
            await dbl.GetTextChannel(265156286406983680).SendMessageAsync(
                $"<@168548441939509248>: I am a SIVA not owned by you. Please do not post SIVA to a bot list again, <@{Config.GetOwner()}>.");
            await dbl.LeaveAsync();
        }

        public async Task Guilds(SocketGuild guild) {
            if (Config.GetBlacklistedOwners().Contains(guild.OwnerId)) {
                await guild.LeaveAsync();
            }
        }

        public async Task HandleMessageOrCommand(SocketMessage s) {
            var msg = (SocketUserMessage) s;
            var ctx = new SIVAContext(_client, msg);
            await ServiceProvider.GetRequiredService<BlacklistService>().CheckMessageForBlacklistedWords(s);
            await ServiceProvider.GetRequiredService<AntilinkService>().CheckMessageForInvite(s);
            await ServiceProvider.GetRequiredService<EconomyService>().Give(ctx);
            //await SupportMessageListener.Check(s);
            if (ctx.User.IsBot) return;
            var config = ServerConfig.Get(ctx.Guild);
            Users.Get(s.Author.Id);
            var prefix = config.CommandPrefix == string.Empty ? Config.GetCommandPrefix() : config.CommandPrefix;

            if (config.EmbedColourR == 0 && config.EmbedColourG == 0 && config.EmbedColourB == 0) {
                config.EmbedColourR = 112;
                config.EmbedColourG = 0;
                config.EmbedColourB = 251;
                ServerConfig.Save();
            }

            var argPos = 0; //i'd get rid of this but because of Discord.Net being finnicky i can't.

            var msgStrip = msg.Content.Replace(prefix, string.Empty);


            if (msg.HasStringPrefix(prefix, ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos)) {
                var result = await _service.ExecuteAsync(ctx, argPos, null);

                if (!result.IsSuccess && result.ErrorReason != "Unknown command.") {
                    string reason;
                    switch (result.ErrorReason) {
                        case "The server responded with error 403: Forbidden":
                            reason =
                                "I'm not allowed to do that. Either I don't have permission or the requested user is higher than me in the role heirarchy.";
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

                if (result.ErrorReason == "Unknown command.") return;

                OnCommand(result, ctx);

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

        private void OnCommand(IResult res, SIVAContext ctx) {
            if (Config.GetLogAllCommands()) {
                if (res.IsSuccess) {
                    _logger.Info($"--|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator}");
                    _logger.Info($"--|     -Command Issued: {ctx.Message.Content}");
                    _logger.Info($"--|           -In Guild: {ctx.Guild.Name}");
                    _logger.Info($"--|         -In Channel: #{ctx.Channel.Name}");
                    _logger.Info($"--|        -Time Issued: {DateTime.Now}");
                    _logger.Info($"--|           -Executed: {res.IsSuccess} ");
                    _logger.Info("-------------------------------------------------");
                }
                else {
                    _logger.Error($"--|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator}");
                    _logger.Error($"--|     -Command Issued: {ctx.Message.Content}");
                    _logger.Error($"--|           -In Guild: {ctx.Guild.Name}");
                    _logger.Error($"--|         -In Channel: #{ctx.Channel.Name}");
                    _logger.Error($"--|        -Time Issued: {DateTime.Now}");
                    _logger.Error($"--|           -Executed: {res.IsSuccess} | Reason: {res.ErrorReason}");
                    _logger.Error("-------------------------------------------------");
                }
            }

            try {
                File.AppendAllText("Commands.log",
                    $"--|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})\n");
                File.AppendAllText("Commands.log",
                    $"--|     -Command Issued: {ctx.Message.Content} ({ctx.Message.Id})\n");
                File.AppendAllText("Commands.log",
                    $"--|           -In Guild: {ctx.Guild.Name} ({ctx.Guild.Id})\n");
                File.AppendAllText("Commands.log",
                    $"--|         -In Channel: #{ctx.Channel.Name} ({ctx.Channel.Id})\n");
                File.AppendAllText("Commands.log", $"--|        -Time Issued: {DateTime.Now}\n");
                File.AppendAllText("Commands.log", res.IsSuccess
                    ? $"--|           -Executed: {res.IsSuccess}\n"
                    : $"--|           -Executed: {res.IsSuccess} | Reason: {res.ErrorReason}\n");
                File.AppendAllText("Commands.log", "-------------------------------------------------\n");
            }
            catch (FileNotFoundException) {
                _logger.Error("The Commands.log file doesn't exist. Creating it now.");
                File.Create("Commands.log");
            }
        }
    }
}