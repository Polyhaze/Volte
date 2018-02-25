using System;
using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;
using System.Reflection;
using DiscordBot.Core.LevelingSystem;
using DiscordBot.Core.UserAccounts;
using System.Linq;
using Discord;
using DiscordBot.Core.Config;

namespace DiscordBot
{
    internal class MessageHandler
    {
        DiscordSocketClient _client;
        CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _client.MessageReceived += HandleCommandAsync;
            _client.MessageReceived += SupportChannelUtils;
            _client.MessageReceived += MassPengChecks;
            _client.UserJoined += Autorole;
            //_client.ChannelUpdated
            //_client.GuildMemberUpdated
            //_client.MessageDeleted
            //_client.RoleCreated
            //_client.UserLeft
            //_client.UserBanned += BannedUser;
            //_client.ChannelCreated
        }

        /*private async Task BannedUser(SocketGuildUser user, SocketGuild guild, Task task)
        {
            var embed = new EmbedBuilder();
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
            if (context.User.IsBot) return;

            Leveling.UserSentMessage((SocketGuildUser)context.User, (SocketTextChannel)context.Channel);

            int argPos = 0;
            if (msg.HasStringPrefix(Config.bot.prefix, ref argPos)
                || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos);
                Console.WriteLine($"\\|-Command from user: {context.User.Username}#{context.User.Discriminator} ({context.User.Id})");
                Console.WriteLine($"\\|   -Command Issued: {msg.Content} ({msg.Id})");
                Console.WriteLine($"\\|         -In Guild: {context.Guild.Name} ({context.Guild.Id})");
                Console.WriteLine($"\\|       -In Channel: #{context.Channel.Name} ({context.Channel.Id})");
                Console.WriteLine($"\\|      -Time Issued: {DateTime.Now}");
                if (result.IsSuccess)
                {
                    Console.WriteLine($"\\|         -Executed: {result.IsSuccess}");
                }
                else
                {
                    Console.WriteLine($"\\|         -Executed: {result.IsSuccess} | Reason: {result.ErrorReason}");
                }
            }
        }

        public async Task MassPengChecks(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            var context = new SocketCommandContext(_client, msg);
            if (context.User.IsBot) return;

            if (msg.Content.Contains("@everyone") || msg.Content.Contains("@here") && context.User != context.Guild.Owner)
            {
                await msg.DeleteAsync();
                await context.Channel.SendMessageAsync($"{msg.Author.Mention}, try not to mass ping.");
            }
        }

        public async Task SupportChannelUtils(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            var context = new SocketCommandContext(_client, msg);
            if (context.User.IsBot) return;

            if (msg.Content == "SetupSupport")
            {
                var config = GuildConfig.GetGuildConfig(context.Guild.Id);
                if (config == null)
                {
                    await context.Channel.SendMessageAsync("In order to use SIVA's Support Feature, you need to make a Support config!\nDo so with the Support config commands, `$h Support`.");
                    return;
                }

                var embed = new EmbedBuilder();
                embed.WithColor(Config.bot.defaultEmbedColour);
                embed.WithDescription(Utilities.GetAlert("SupportEmbedText"));
                embed.WithAuthor(context.Guild.Owner);
                await context.Channel.SendMessageAsync("", false, embed);
                config.SupportChannelId = context.Channel.Id;
                config.SupportChannelName = context.Channel.Name;

            }

            if (msg.Content != "SetupSupport")
            {
                var supportConfig = GuildConfig.GetGuildConfig(context.Guild.Id);
                var supportStartChannel = context.Guild.Channels.FirstOrDefault(c => c.Name == supportConfig.SupportChannelName);

                if (msg.Channel == supportStartChannel)
                {
                    //var categoryId = supportConfig.SupportCategoryId;
                    var supportChannelExists = context.Guild.Channels.FirstOrDefault(c => c.Name == $"{supportConfig.SupportChannelName}-{context.User.Id}");
                    var role = context.Guild.Roles.FirstOrDefault(r => r.Name == supportConfig.SupportRole);

                    if (supportChannelExists == null)
                    {
                        await msg.DeleteAsync();
                        var channel = await context.Guild.CreateTextChannelAsync($"{supportConfig.SupportChannelName}-{context.User.Id}");
                        await channel.AddPermissionOverwriteAsync(context.User, OverwritePermissions.AllowAll(channel));
                        await channel.AddPermissionOverwriteAsync(context.Guild.EveryoneRole, OverwritePermissions.DenyAll(channel));
                        await channel.AddPermissionOverwriteAsync(role, OverwritePermissions.AllowAll(channel));
                        var embed = new EmbedBuilder();
                        embed.WithAuthor(context.User);
                        embed.WithThumbnailUrl(context.User.GetAvatarUrl());
                        embed.AddInlineField("What do you need help with?", $"{msg.Content}");
                        embed.WithColor(Config.bot.defaultEmbedColour);
                        embed.WithFooter($"Time Created: {DateTime.Now}");
                        await channel.SendMessageAsync($"You can close this ticket if you have the role set for moderating tickets: `{supportConfig.SupportRole}`", false, embed);

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
