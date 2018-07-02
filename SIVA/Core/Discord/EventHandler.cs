using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Discord;
using SIVA.Core.Discord.Automation;
using SIVA.Core.Discord.Support;
using SIVA.Core.Files.Readers;

namespace SIVA.Core.Discord
{
    public class EventHandler
    {
        private DiscordSocketClient _client;
        private CommandService _service;

        public async Task Init()
        {
            _client = DiscordLogin.Client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _service.Log += DiscordLogin.Log;
            _client.MessageReceived += HandleMessageOrCommand;
            _client.JoinedGuild += Guilds;
            _client.UserJoined += Autorole;
            _client.MessageReceived += SupportMessageListener.Check;
        }

        public async Task Autorole(SocketGuildUser user)
        {
            var config = ServerConfig.Get(user.Guild);
            if (!string.IsNullOrEmpty(config.Autorole))
            {
                var targetRole = user.Guild.Roles.FirstOrDefault(r => r.Name == config.Autorole);
                await user.AddRoleAsync(targetRole);
            }
        }

        public async Task Guilds(SocketGuild guild)
        {
            if (Config.conf.BlacklistedServerOwners.Contains(guild.OwnerId))
            {
                await guild.LeaveAsync();
            }
        }

        public async Task HandleMessageOrCommand(SocketMessage s)
        {
            var msg = (SocketUserMessage)s;
            var ctx = new SocketCommandContext(_client, msg);
            await Blacklist.CheckMessageForBlacklistedWords(s);
            await Antilink.CheckMessageForInvite(s);
            if (ctx.User.IsBot) return;
            var config = ServerConfig.Get(ctx.Guild);
            Users.Get(s.Author.Id);
            var prefix = config.CommandPrefix ?? Config.conf.CommandPrefix;

            if (config.EmbedColourR == 0 && config.EmbedColourG == 0 && config.EmbedColourB == 0)
            {
                config.EmbedColourR = 112;
                config.EmbedColourG = 0;
                config.EmbedColourB = 251;
                ServerConfig.Save();
            }

            var argPos = 0;

            var msgStrip = ctx.Message.Content.Replace(prefix, string.Empty);

            foreach (var command in config.CustomCommands)
            {
                if (msg.HasStringPrefix(prefix + command.Key, ref argPos))
                {
                    await ctx.Channel.SendMessageAsync(command.Value);
                }
            }

            if (msg.HasStringPrefix(prefix, ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _service.ExecuteAsync(ctx, argPos);
                
                if (result.IsSuccess == false && result.ErrorReason != "Unknown command.")
                {
                    string reason;
                    switch (result.ErrorReason)
                    {
                        case "The server responded with error 403: Forbidden":
                            reason =
                                "I'm not allowed to do that. Either I don't have permission or the requested user is higher than me in the role heirarchy.";
                            break;
                        case "Sequence contains no elements":
                            try
                            {
                                reason = $"{msg.MentionedUsers.FirstOrDefault().Mention} doesn't have any.";
                            }
                            catch (NullReferenceException)
                            {
                                reason = "List has no elements.";
                            }

                            break;
                        case "Failed to parse Boolean":
                            reason = "You can only input `true` or `false` for this command.";
                            break;
                        default:
                            reason = result.ErrorReason;
                            break;
                    }

                    var embed = new EmbedBuilder();

                    if (msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
                    {
                        var nm = msg.Content.Replace($"<@{_client.CurrentUser.Id}> ", config.CommandPrefix);
                        embed.AddField("Error in command:", nm);
                        embed.AddField("Error reason:", reason);
                        embed.AddField("Weird error?",
                            "[Report it in the SIVA-dev server](https://discord.gg/prR9Yjq)");
                        embed.WithAuthor(ctx.User);
                        embed.WithColor(Config.conf.ErrorEmbedColour);
                        await ctx.Channel.SendMessageAsync("", false, embed.Build());
                    }
                    else
                    {
                        var nm = msg.Content;
                        embed.AddField("Error in command:", nm);
                        embed.AddField("Error reason:", reason);
                        embed.AddField("Weird error?",
                            "[Report it in the SIVA-dev server](https://discord.gg/prR9Yjq)");
                        embed.WithAuthor(ctx.User);
                        embed.WithColor(Config.conf.ErrorEmbedColour);
                        await ctx.Channel.SendMessageAsync("", false, embed.Build());
                    }
                }

                if (result.ErrorReason == "Unknown command.") return;

                if (Config.conf.LogAllCommands)
                {
                    Console.WriteLine($"--|  -Command from user: {ctx.User.Username}#{ctx.User.Discriminator}");
                    Console.WriteLine($"--|     -Command Issued: {msg.Content}");
                    Console.WriteLine($"--|           -In Guild: {ctx.Guild.Name}");
                    Console.WriteLine($"--|         -In Channel: #{ctx.Channel.Name}");
                    Console.WriteLine($"--|        -Time Issued: {DateTime.Now}");
                    Console.WriteLine(result.IsSuccess
                        ? $"--|           -Executed: {result.IsSuccess}"
                        : $"--|           -Executed: {result.IsSuccess} | Reason: {result.ErrorReason}");
                    Console.WriteLine("-------------------------------------------------");
                }
                
                try
                {
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
                catch (FileNotFoundException)
                {
                    Console.WriteLine("The Commands.log file wasn't found, creating it now.");
                    File.WriteAllText("Commands.log", "");
                }
                
                if (config.DeleteMessageOnCommand)
                {
                    await ctx.Message.DeleteAsync();
                }
            }
            else
            {
                if (msg.Content.Contains($"<@{_client.CurrentUser.Id}>"))
                {
                    await ctx.Channel.SendMessageAsync("<:whO_PENG:437088256291504130>");
                }
            }
        }
    }
}