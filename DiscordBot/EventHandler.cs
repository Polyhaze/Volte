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

namespace SIVA
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
            _client.MessageReceived += SupportChannelUtils;
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
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
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

            var config = GuildConfig.GetGuildConfig(context.Guild.Id);

            if (config.Leveling)
            {
                Leveling.UserSentMessage((SocketGuildUser)context.User, (SocketTextChannel)context.Channel);
            }

            var prefix = SIVA.Config.bot.Prefix;

            if (config.CommandPrefix != SIVA.Config.bot.Prefix)
            {
                prefix = config.CommandPrefix;
            }

            int argPos = 0;
            if (msg.HasStringPrefix(prefix, ref argPos)
                || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos);
                if (result.IsSuccess == false && result.ErrorReason != "Unknown command.")
                {

                    var embed = new EmbedBuilder();
                    embed.WithColor(SIVA.Config.bot.ErrorEmbedColour);
                    embed.WithFooter("Seems like a weird error? Report it in the SIVA-dev server!");

                    if (msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
                    {
                        var nm = msg.Content.Replace($"<@{_client.CurrentUser.Id}> ", config.CommandPrefix);
                        embed.WithDescription($"Error in command: {nm}\n\nReason: {result.ErrorReason}");
                        await context.Channel.SendMessageAsync("", false, embed);
                    }
                    else
                    {
                        var nm = msg.Content;
                        embed.WithDescription($"Error in command: {nm}\n\nReason: {result.ErrorReason}");
                        await context.Channel.SendMessageAsync("", false, embed);
                    }
                }
                Console.WriteLine($"\\|  -Command from user: {context.User.Username}#{context.User.Discriminator} ({context.User.Id})");
                Console.WriteLine($"\\|     -Command Issued: {msg.Content} ({msg.Id})");
                Console.WriteLine($"\\|           -In Guild: {context.Guild.Name} ({context.Guild.Id})");
                Console.WriteLine($"\\|         -In Channel: #{context.Channel.Name} ({context.Channel.Id})");
                Console.WriteLine($"\\|        -Time Issued: {DateTime.Now}");
                Console.WriteLine(result.IsSuccess
                    ? $"\\|           -Executed: {result.IsSuccess}"
                    : $"\\|           -Executed: {result.IsSuccess} | Reason: {result.ErrorReason}");

                if (!File.Exists("Commands.log"))
                {
                    File.WriteAllText("Commands.log", "");
                }

                File.AppendAllText("Commands.log", $"\\|  -Command from user: {context.User.Username}#{context.User.Discriminator} ({context.User.Id})\n");
                File.AppendAllText("Commands.log", $"\\|     -Command Issued: {msg.Content} ({msg.Id})\n");
                File.AppendAllText("Commands.log", $"\\|           -In Guild: {context.Guild.Name} ({context.Guild.Id})\n");
                File.AppendAllText("Commands.log", $"\\|         -In Channel: #{context.Channel.Name} ({context.Channel.Id})\n");
                File.AppendAllText("Commands.log", $"\\|        -Time Issued: {DateTime.Now}\n");
                File.AppendAllText("Commands.log", result.IsSuccess
                    ? $"\\|           -Executed: {result.IsSuccess}\n-------------------------------------------------\n"
                    : $"\\|           -Executed: {result.IsSuccess} | Reason: {result.ErrorReason}\n-------------------------------------------------\n");
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
                embed.WithFooter($"User ID: {s.Id} | Guild ID: {s.Guild.Id} | Guild Owner: {s.Guild.Owner.Username}#{s.Guild.Owner.Discriminator}");
                embed.WithThumbnailUrl(s.Guild.IconUrl);
                await channel.SendMessageAsync("", false, embed);

            }
        }

        public async Task Welcome(SocketGuildUser s)
        {
            var config = GuildConfig.GetGuildConfig(s.Guild.Id);

            if (config.WelcomeChannel != 0)
            {
                var rmsg = config.WelcomeMessage.Replace("{UserMention}", s.Mention);
                var msg = rmsg.Replace("{ServerName}", s.Guild.Name);

                var channel = s.Guild.GetTextChannel(config.WelcomeChannel);
                var embed = new EmbedBuilder();
                embed.WithDescription(msg);
                embed.WithColor(new Color(config.WelcomeColour1, config.WelcomeColour2, config.WelcomeColour3));
                embed.WithFooter($"User ID: {s.Id} | Guild ID: {s.Guild.Id} | Guild Owner: {s.Guild.Owner.Username}#{s.Guild.Owner.Discriminator}");
                embed.WithThumbnailUrl(s.Guild.IconUrl);
                await channel.SendMessageAsync("", false, embed);
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

        public async Task SupportChannelUtils(SocketMessage s)
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
                embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
                embed.WithDescription(Utilities.GetLocaleMsg("SupportEmbedText"));
                embed.WithAuthor(context.Guild.Owner);
                await context.Channel.SendMessageAsync("", false, embed);
                config.SupportChannelId = context.Channel.Id;
                config.SupportChannelName = context.Channel.Name;
                config.CanCloseOwnTicket = true;

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
                        var channel = context.Guild.GetTextChannel(chnl.Id);
                        await channel.AddPermissionOverwriteAsync(context.User, OverwritePermissions.AllowAll(channel));
                        await channel.AddPermissionOverwriteAsync(context.Guild.EveryoneRole, OverwritePermissions.DenyAll(channel));
                        await channel.AddPermissionOverwriteAsync(role, OverwritePermissions.AllowAll(channel));
                        var embed = new EmbedBuilder();
                        embed.WithAuthor(msg.Author);
                        embed.WithThumbnailUrl(context.User.GetAvatarUrl());
                        embed.WithDescription($"What do you need help with?\n```{msg.Content}```");
                        embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
                        embed.WithFooter($"Time Created: {DateTime.Now}");
                        await channel.SendMessageAsync($"You can close this ticket if you have the role set for moderating tickets: `{supportConfig.SupportRole}`");
                        await channel.SendMessageAsync("", false, embed);

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
