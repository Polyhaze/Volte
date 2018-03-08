using System;
using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;
using System.Reflection;
using SIVA.Core.LevelingSystem;
using System.Linq;
using Discord;
using SIVA.Core.Config;
using System.IO;
using SIVA.Core.UserAccounts;

namespace SIVA.Core.Bot
{
    internal class EventHandler
    {
        private DiscordSocketClient _client;
        private CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _client.MessageReceived += MassPengChecks;
            _client.MessageReceived += HandleCommandAsync;
            _client.MessageReceived += SupportSystem;
            _client.UserJoined += Welcome;
            _client.UserJoined += Autorole;
            _client.JoinedGuild += GuildUtils;
            _client.UserLeft += Goodbye;
        }

        /*private async Task BannedUser(SocketGuildUser user, SocketGuild guild)
        {
            var config = GuildConfig.GetGuildConfig(guild.Id);
            if (config == null) return;

            var chnl = guild.GetTextChannel(config.ChannelId);

            var embed = new EmbedBuilder();
            embed.WithColor(Bot.Config.bot.DefaultEmbedColour);
            embed.WithDescription($"User Banned: {user.Username} - {user.Nickname}");
            embed.WithThumbnailUrl("https://yt3.ggpht.com/a-/AJLlDp3QNvGtiRpzGAvxRx0xQLpjOw1I_knKVT9NJA=s900-mo-c-c0xffffffff-rj-k-no");
            await chnl.SendMessageAsync("", false, embed);


        }*/

        private async Task Autorole(SocketGuildUser user)
        {
            var config = GuildConfig.GetGuildConfig(user.Guild.Id);
            if (config.RoleToApply != null || config.RoleToApply != "")
            {
                var targetRole = user.Guild.Roles.FirstOrDefault(r => r.Name == config.RoleToApply);
                await user.AddRoleAsync(targetRole);
            }
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            var context = new SocketCommandContext(_client, msg);

            var config = GuildConfig.GetGuildConfig(context.Guild.Id) ?? GuildConfig.CreateGuildConfig(context.Guild.Id);
            config.GuildOwnerId = context.Guild.Owner.Id;
            GuildConfig.SaveGuildConfig();

            if (config.Leveling)
            {
                Leveling.UserSentMessage((SocketGuildUser)context.User, (SocketTextChannel)context.Channel);
            }

            if (context.Guild.Id == 385902350432206849)
            {
                if (msg.Content.Contains("🎷") || msg.Content.Contains("🎺"))
                {
                    if (msg.Author.Id == 360493978371751937)
                    {
                        await msg.DeleteAsync();
                        await context.Channel.SendMessageAsync(context.User.Mention + " no");
                    }
                }
            }

            var prefix = Config.bot.Prefix;

            if (config.CommandPrefix != Config.bot.Prefix)
            {
                prefix = config.CommandPrefix;
            }

            try //attempt something
            {
                if (config.Antilink == false)
                {
                    //if antilink is turned off then proceed to processing the command
                }
                else //if it isnt then do the following
                {
                    if (msg.Author.Id == config.GuildOwnerId) //if the message is from the guild owner
                    {
                        //don't do anything
                    }
                    else //if the message isnt from the guild owner, do the following
                    {
                        if (msg.Content.Contains("https://discord.gg")) //if the message contains https://discord.gg (it's an invite link), then delete it
                        {
                            var offendingAccount = UserAccounts.UserAccounts.GetAccount(context.User);
                            offendingAccount.Warns.Add($"Invite link at {DateTime.Now}");
                            offendingAccount.WarnCount++;
                            UserAccounts.UserAccounts.SaveAccounts();
                            await msg.DeleteAsync();
                            var embed = new EmbedBuilder();
                            embed.WithDescription($"{context.User.Mention}, no invite links.");
                            embed.WithColor(Config.bot.DefaultEmbedColour);
                            await context.Channel.SendMessageAsync("", false, embed);
                        }
                    }
                }
            }
            catch (NullReferenceException) // if the config variable returns an invalid value then create the guild config
            {
                GuildConfig.CreateGuildConfig(context.Guild.Id);
            }

            int argPos = 0;
            if (msg.HasStringPrefix(prefix, ref argPos)
                || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos);
                if (result.IsSuccess == false && result.ErrorReason != "Unknown command.")
                {
                    string reason = result.ErrorReason;
                    if (result.ErrorReason == "The server responded with error 403: Forbidden")
                    {
                        reason = "I'm not allowed to do that. (Missing permission, most likely.)";
                    }

                    var embed = new EmbedBuilder();
                    embed.WithColor(Config.bot.ErrorEmbedColour);
                    embed.WithFooter("Seems like a weird error? Report it in the SIVA-dev server!");

                    if (msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
                    {
                        var nm = msg.Content.Replace($"<@{_client.CurrentUser.Id}> ", config.CommandPrefix);
                        embed.WithDescription($"Error in command: {nm}\n\nReason: {reason}");
                        await context.Channel.SendMessageAsync("", false, embed);
                    }
                    else
                    {
                        var nm = msg.Content;
                        embed.WithDescription($"Error in command: {nm}\n\nReason: {reason}");
                        await context.Channel.SendMessageAsync("", false, embed);
                    }
                }
                Console.WriteLine($"\\|  -Command from user: {context.User.Username}#{context.User.Discriminator}");
                Console.WriteLine($"\\|     -Command Issued: {msg.Content}");
                Console.WriteLine($"\\|           -In Guild: {context.Guild.Name}");
                Console.WriteLine($"\\|         -In Channel: #{context.Channel.Name}");
                Console.WriteLine($"\\|        -Time Issued: {DateTime.Now}");
                Console.WriteLine(result.IsSuccess
                    ? $"\\|           -Executed: {result.IsSuccess}"
                    : $"\\|           -Executed: {result.IsSuccess} | Reason: {result.ErrorReason}");
                try 
                {
                    File.AppendAllText("Commands.log", $"\\|  -Command from user: {context.User.Username}#{context.User.Discriminator} ({context.User.Id})\n");
                    File.AppendAllText("Commands.log", $"\\|     -Command Issued: {msg.Content} ({msg.Id})\n");
                    File.AppendAllText("Commands.log", $"\\|           -In Guild: {context.Guild.Name} ({context.Guild.Id})\n");
                    File.AppendAllText("Commands.log", $"\\|         -In Channel: #{context.Channel.Name} ({context.Channel.Id})\n");
                    File.AppendAllText("Commands.log", $"\\|        -Time Issued: {DateTime.Now}\n");
                    File.AppendAllText("Commands.log", result.IsSuccess
                        ? $"\\|           -Executed: {result.IsSuccess}\n-------------------------------------------------\n"
                        : $"\\|           -Executed: {result.IsSuccess} | Reason: {result.ErrorReason}\n-------------------------------------------------\n");
                }
                catch (FileNotFoundException) 
                {
                    Console.WriteLine("The Commands.log file wasn't found, creating it now.");
                    File.WriteAllText("Commands.log", "");
                }
            }
        }

        public async Task Goodbye(SocketGuildUser s)
        {
            var config = GuildConfig.GetGuildConfig(s.Guild.Id);

            if (config.WelcomeChannel != 0)
            {
                var rmsg = config.LeavingMessage.Replace("{UserMention}", $"<@{s.Id}>");
                var msg = rmsg.Replace("{ServerName}", s.Guild.Name);

                var channel = s.Guild.GetTextChannel(config.WelcomeChannel);
                var embed = new EmbedBuilder();
                embed.WithDescription(msg);
                embed.WithColor(new Color(config.WelcomeColour1, config.WelcomeColour2, config.WelcomeColour3));
                embed.WithFooter($"Guild Owner: {s.Guild.Owner.Username}#{s.Guild.Owner.Discriminator}");
                embed.WithThumbnailUrl(s.Guild.IconUrl);
                await channel.SendMessageAsync("", false, embed);

            }
        }

        public async Task Welcome(SocketGuildUser user)
        {
            var config = GuildConfig.GetGuildConfig(user.Guild.Id);

            if (config.WelcomeChannel != 0)
            {
                var rmsg = config.WelcomeMessage.Replace("{UserMention}", user.Mention);
                var msg = rmsg.Replace("{ServerName}", user.Guild.Name);

                var channel = user.Guild.GetTextChannel(config.WelcomeChannel);
                var embed = new EmbedBuilder();
                embed.WithDescription(msg);
                embed.WithColor(new Color(config.WelcomeColour1, config.WelcomeColour2, config.WelcomeColour3));
                embed.WithFooter($"Guild Owner: {user.Guild.Owner.Username}#{user.Guild.Owner.Discriminator}");
                embed.WithThumbnailUrl(user.Guild.IconUrl);
                await channel.SendMessageAsync("", false, embed);
            }

            if (user.Guild.Id == 419612620090245140)
            {
                await user.ModifyAsync(x => 
                {
                    x.Nickname = $"{user.Username}.cs";
                });
            }
        }

        public async Task GuildUtils(SocketGuild s)
        {

            var config = GuildConfig.GetGuildConfig(s.Id) ??
                         GuildConfig.CreateGuildConfig(s.Id);

            if (s.Owner.Id == 396003871434211339)
            {
                await s.LeaveAsync();
            }

            var owner = s.Owner;
            var dmChannel = await owner.GetOrCreateDMChannelAsync();
            var embed = new EmbedBuilder();
            embed.WithTitle($"Thanks for adding me to your server, {s.Owner.Username}!");
            embed.WithDescription($"For quick information, visit the wiki: https://github.com/Greeem/greeem.github.io/wiki \nNeed quick help? Visit the SIVA-dev server and create a support ticket: https://discord.io/SIVA \nTo get started, use the command `$h`. Follow that with a module to get a list of commands!");
            embed.WithThumbnailUrl(s.Owner.GetAvatarUrl());
            embed.WithFooter("Still need help? Visit the SIVA-dev server linked above.");
            embed.WithColor(Config.bot.DefaultEmbedColour);

            await dmChannel.SendMessageAsync("", false, embed);

            config.GuildOwnerId = s.Owner.Id;
            GuildConfig.SaveGuildConfig();

        }

        public async Task MassPengChecks(SocketMessage s)
        {

            var msg = s as SocketUserMessage;
            var context = new SocketCommandContext(_client, msg);
            if (context.User.IsBot) return;

            var config = GuildConfig.GetGuildConfig(context.Guild.Id);

            if (config.MassPengChecks == true)
            {
                if (msg.Content.Contains("@everyone") || msg.Content.Contains("@here"))
                {
                    if (msg.Author != context.Guild.Owner)
                    {
                        await msg.DeleteAsync();
                        await context.Channel.SendMessageAsync($"{msg.Author.Mention}, try not to mass ping.");
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                return;
            }
        }

        public async Task SupportSystem(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            var context = new SocketCommandContext(_client, msg);
            if (context.User.IsBot) return;

            var config = GuildConfig.GetGuildConfig(context.Guild.Id) ??
                         GuildConfig.CreateGuildConfig(context.Guild.Id);
            config.GuildOwnerId = context.Guild.OwnerId;

            if (msg.Content == "SetupSupport" && msg.Author.Id == config.GuildOwnerId)
            {
                var embed = new EmbedBuilder();
                embed.WithColor(Config.bot.DefaultEmbedColour);
                embed.WithDescription(Utilities.GetLocaleMsg("SupportEmbedText"));
                embed.WithAuthor(context.Guild.Owner);
                await context.Channel.SendMessageAsync("", false, embed);
                config.SupportChannelId = context.Channel.Id;
                config.SupportChannelName = context.Channel.Name;
                config.CanCloseOwnTicket = true;
                GuildConfig.SaveGuildConfig();

            }
            
            if (msg.Content != "SetupSupport")
            {
                var supportConfig = GuildConfig.GetGuildConfig(context.Guild.Id);
                var supportStartChannel = context.Guild.Channels.FirstOrDefault(c => c.Name == supportConfig.SupportChannelName);

                if (msg.Channel == supportStartChannel)
                {
                    var supportChannelExists = context.Guild.Channels.FirstOrDefault(c => c.Name == $"{supportConfig.SupportChannelName}-{context.User.Id}");
                    var role = context.Guild.Roles.FirstOrDefault(r => r.Name == supportConfig.SupportRole);

                    if (supportChannelExists == null)
                    {
                        await msg.DeleteAsync();
                        var chnl = await context.Guild.CreateTextChannelAsync($"{supportConfig.SupportChannelName}-{context.User.Id}");
                        await chnl.AddPermissionOverwriteAsync(context.User, OverwritePermissions.AllowAll(chnl));
                        await chnl.AddPermissionOverwriteAsync(context.Guild.EveryoneRole, OverwritePermissions.DenyAll(chnl));
                        if (role != null)
                        {
                            await chnl.AddPermissionOverwriteAsync(role, OverwritePermissions.AllowAll(chnl));
                        }
                        
                        await chnl.ModifyAsync(x =>
                        {
                            x.Position = supportStartChannel.Position - 1;
                            x.Topic = $"Support ticket created by <@{msg.Author.Id}> at {DateTime.UtcNow} (UTC)";
                        });
                        var embed = new EmbedBuilder();
                        embed.WithAuthor(msg.Author);
                        embed.WithThumbnailUrl(context.User.GetAvatarUrl());
                        embed.WithDescription($"What do you need help with?\n```{msg.Content}```");
                        embed.WithColor(Config.bot.DefaultEmbedColour);
                        embed.WithFooter($"Time Created: {DateTime.Now}");
                        await chnl.SendMessageAsync($"You can close this ticket if you have the role set for moderating tickets: `{supportConfig.SupportRole}`");
                        await chnl.SendMessageAsync("", false, embed);

                    }
                    else
                    {
                        var channel = context.Guild.GetTextChannel(supportChannelExists.Id);
                        await channel.SendMessageAsync($"{context.User.Mention}, please send your message here rather than the primary support channel. Text: ```{msg.Content}``` If you cannot type in here, please tell an admin.");
                        await msg.DeleteAsync();
                    }
                }
            }
        }
    }
}
