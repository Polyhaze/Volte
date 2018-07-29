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
using SIVA.Core.Discord.Support;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Discord {
    public class SivaHandler {
        private DiscordSocketClient _client;
        private CommandService _service;

        public IServiceProvider BuildServiceProvider() => new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_service)
            .AddSingleton<SivaHandler>()
            .BuildServiceProvider();


        public async Task Init() {
            var config = new CommandServiceConfig {
                IgnoreExtraArgs = true,
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false,
                LogLevel = LogSeverity.Verbose
            };
            _client = Siva.GetInstance();
            _service = new CommandService(config);
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), BuildServiceProvider());
            _client.MessageReceived += HandleMessageOrCommand;
            _client.JoinedGuild += Guilds;
            _client.UserJoined += new Welcome().Join;
            _client.UserJoined += new Autorole().Apply;
            _client.UserLeft += new Welcome().Leave;
            _client.Ready += OnReady;
        }

        private async Task OnReady() {
            var dbl = Siva.GetInstance().GetGuild(264445053596991498);
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
            var ctx = new SocketCommandContext(_client, msg);
            await new Blacklist().CheckMessageForBlacklistedWords(s);
            await new Antilink().CheckMessageForInvite(s);
            await SupportMessageListener.Check(s);
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
                var result = await _service.ExecuteAsync(ctx, argPos, BuildServiceProvider());

                if (result.IsSuccess == false && result.ErrorReason != "Unknown command.") {
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

                if (Config.GetLogAllCommands()) {
                    if (result.IsSuccess.Equals(false)) {
                        Console.WriteLine($"--|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator}");
                        Console.WriteLine($"--|     -Command Issued: {msg.Content}");
                        Console.WriteLine($"--|           -In Guild: {ctx.Guild.Name}");
                        Console.WriteLine($"--|         -In Channel: #{ctx.Channel.Name}");
                        Console.WriteLine($"--|        -Time Issued: {DateTime.Now}");
                        Console.WriteLine(
                            $"--|           -Executed: {result.IsSuccess} | Reason: {result.ErrorReason}");
                        Console.WriteLine("-------------------------------------------------");
                    }
                    else {
                        Console.WriteLine($"--|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator}");
                        Console.WriteLine($"--|     -Command Issued: {msg.Content}");
                        Console.WriteLine($"--|           -In Guild: {ctx.Guild.Name}");
                        Console.WriteLine($"--|         -In Channel: #{ctx.Channel.Name}");
                        Console.WriteLine($"--|        -Time Issued: {DateTime.Now}");
                        Console.WriteLine($"--|           -Executed: {result.IsSuccess}");
                        Console.WriteLine("-------------------------------------------------");
                    }
                }

                try {
                    File.AppendAllText("Commands.log",
                        $"--|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})\n");
                    File.AppendAllText("Commands.log", $"--|     -Command Issued: {msg.Content} ({msg.Id})\n");
                    File.AppendAllText("Commands.log",
                        $"--|           -In Guild: {ctx.Guild.Name} ({ctx.Guild.Id})\n");
                    File.AppendAllText("Commands.log",
                        $"--|         -In Channel: #{ctx.Channel.Name} ({ctx.Channel.Id})\n");
                    File.AppendAllText("Commands.log", $"--|        -Time Issued: {DateTime.Now}\n");
                    File.AppendAllText("Commands.log", result.IsSuccess
                        ? $"--|           -Executed: {result.IsSuccess}\n"
                        : $"--|           -Executed: {result.IsSuccess} | Reason: {result.ErrorReason}\n");
                    File.AppendAllText("Commands.log", "-------------------------------------------------\n");
                }
                catch (FileNotFoundException) {
                    Console.WriteLine("The Commands.log file wasn't found, creating it now.");
                    File.Create("Commands.log");
                }

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