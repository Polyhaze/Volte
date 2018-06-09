using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.JsonFiles;

namespace SIVA.Core.Bot.Internal
{
    internal class EventHandler
    {
        private DiscordSocketClient _client = Program._client;
        private CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _service.Log += EventUtils.Log;
            _client.MessageReceived += HandleCommandAsync;
            _client.ReactionAdded += Support.DeleteSupportChannel;
            _client.UserJoined += EventUtils.Welcome;
            _client.UserJoined += EventUtils.Autorole;
            _client.JoinedGuild += EventUtils.GuildUtils;
            _client.UserLeft += EventUtils.Goodbye;
            _client.UserBanned += Logging.HandleBans;
            _client.ChannelCreated += Logging.HandleChannelCreate;
            _client.ChannelDestroyed += Logging.HandleChannelDelete;
            _client.GuildUpdated += Logging.HandleServerUpdate;
            _client.MessageDeleted += Logging.HandleMessageDelete;
            _client.MessageUpdated += Logging.HandleMessageUpdate;
            _client.UserUpdated += Logging.HandleUserUpdate;
            _client.RoleCreated += Logging.HandleRoleCreation;
            _client.RoleUpdated += Logging.HandleRoleUpdate;
            _client.RoleDeleted += Logging.HandleRoleDelete;
            _client.Ready += OnClientReady;
        }

        private async Task OnClientReady()
        {
            await DblServerCount.UpdateServerCount(_client);
        }


        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            var context = new SocketCommandContext(_client, msg);
            await EventUtils.AssholeChecks(s);
            await EventUtils.HandleMessages(s);
            await Blacklist.CheckMessageForBlacklistedTerms(s);
            await Support.SupportSystem(s);
            if (msg == null)
            {
                Console.WriteLine($"{s} not cared for as it's null (for whatever reason)");
                return;
            }


            if (context.User.IsBot) return;

            var config = GuildConfig.GetGuildConfig(context.Guild.Id) ??
                         GuildConfig.CreateGuildConfig(context.Guild.Id);
            var prefix = config.CommandPrefix ?? Config.bot.Prefix;

            if (config.EmbedColour1 == 0 && config.EmbedColour2 == 0 && config.EmbedColour3 == 0)
            {
                config.EmbedColour1 = 112;
                config.EmbedColour2 = 0;
                config.EmbedColour3 = 251;
                GuildConfig.SaveGuildConfig();
            }

            var argPos = 0;

            foreach (var command in config.CustomCommands)
                if (msg.HasStringPrefix($"{config.CommandPrefix}{command.Key}", ref argPos))
                {
                    await context.Channel.SendMessageAsync(command.Value);
                    break;
                }

            if (msg.HasStringPrefix(prefix, ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos);
                //Console.WriteLine($"Command -{msg.Content}- executed");
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
                        embed.WithAuthor(context.User);
                        embed.WithColor(Config.bot.ErrorEmbedColour);
                        await context.Channel.SendMessageAsync("", false, embed);
                    }
                    else
                    {
                        var nm = msg.Content;
                        embed.AddField("Error in command:", nm);
                        embed.AddField("Error reason:", reason);
                        embed.AddField("Weird error?",
                            "[Report it in the SIVA-dev server](https://discord.gg/prR9Yjq)");
                        embed.WithAuthor(context.User);
                        embed.WithColor(Config.bot.ErrorEmbedColour);
                        await context.Channel.SendMessageAsync("", false, embed);
                    }
                }

                if (result.ErrorReason == "Unknown command.") return;

                Console.WriteLine($"--|  -Command from user: {context.User.Username}#{context.User.Discriminator}");
                Console.WriteLine($"--|     -Command Issued: {msg.Content}");
                Console.WriteLine($"--|           -In Guild: {context.Guild.Name}");
                Console.WriteLine($"--|         -In Channel: #{context.Channel.Name}");
                Console.WriteLine($"--|        -Time Issued: {DateTime.Now}");
                Console.WriteLine(result.IsSuccess
                    ? $"--|           -Executed: {result.IsSuccess}"
                    : $"--|           -Executed: {result.IsSuccess} | Reason: {result.ErrorReason}");
                Console.WriteLine("-------------------------------------------------");
                try
                {
                    File.AppendAllText("Commands.log",
                        $"--|  -Command from user: {context.User.Username}#{context.User.Discriminator} ({context.User.Id})\n");
                    File.AppendAllText("Commands.log", $"--|     -Command Issued: {msg.Content} ({msg.Id})\n");
                    File.AppendAllText("Commands.log",
                        $"--|           -In Guild: {context.Guild.Name} ({context.Guild.Id})\n");
                    File.AppendAllText("Commands.log",
                        $"--|         -In Channel: #{context.Channel.Name} ({context.Channel.Id})\n");
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
            }
        }
    }
}