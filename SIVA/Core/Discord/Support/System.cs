using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Support {
    internal static class System {
        public static async Task SupportSystem(SocketMessage s) {
            var msg = (SocketUserMessage) s;
            if (msg == null) return;
            var context = new SocketCommandContext(new DiscordSocketClient(), msg);
            if (context.User.IsBot) return;

            var config = ServerConfig.Get(context.Guild);
            config.GuildOwnerId = context.Guild.OwnerId;

            if (msg.Content.ToLower() == "setupsupport" && UserUtils.IsAdmin(context)) {
                var embed = new EmbedBuilder();
                embed.WithColor(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB);
                embed.WithDescription(
                    "To create a support ticket, send a message into this channel. Support tickets will be placed under the " +
                    $"**{SIVA.GetInstance.GetGuild(context.Guild.Id).GetTextChannel(context.Channel.Id).Category.Name}** " +
                    "channel category.");
                embed.WithAuthor(context.Guild.Owner);
                await context.Channel.SendMessageAsync("", false, embed.Build());
                config.SupportChannelId = context.Channel.Id;
                config.SupportChannelName = context.Channel.Name;
                config.SupportCategoryId = ((INestedChannel) context.Channel).CategoryId;
                ServerConfig.Save();
            }

            if (msg.Content.ToLower() != "setupsupport") {
                var supportConfig = ServerConfig.Get(context.Guild);
                var supportStartChannel =
                    context.Guild.Channels.FirstOrDefault(c => c.Name == supportConfig.SupportChannelName);

                if (msg.Channel == supportStartChannel) {
                    var supportChannelExists = context.Guild.Channels.FirstOrDefault(c =>
                        c.Name == $"{supportConfig.SupportChannelName}-{context.User.Id}");
                    var role = context.Guild.Roles.FirstOrDefault(r => r.Name == supportConfig.SupportRole);

                    if (supportChannelExists == null) {
                        await msg.DeleteAsync();
                        var chnl = await context.Guild.CreateTextChannelAsync(
                            $"{supportConfig.SupportChannelName}-{context.User.Id}");
                        await chnl.AddPermissionOverwriteAsync(context.User,
                            new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow,
                                addReactions: PermValue.Allow, sendTTSMessages: PermValue.Deny));
                        await chnl.AddPermissionOverwriteAsync(context.Guild.EveryoneRole,
                            new OverwritePermissions(viewChannel: PermValue.Deny, sendMessages: PermValue.Deny));
                        if (role != null)
                            await chnl.AddPermissionOverwriteAsync(role, OverwritePermissions.AllowAll(chnl));

                        await chnl.ModifyAsync(c => {
                            c.CategoryId = config.SupportCategoryId;
                            c.Topic = $"Support ticket created by <@{msg.Author.Id}> at {DateTime.UtcNow} (UTC)";
                        });
                        var embed = new EmbedBuilder()
                            .WithAuthor(msg.Author)
                            .WithThumbnailUrl(context.User.GetAvatarUrl())
                            .WithTitle("What do you need help with?")
                            .WithDescription(
                                $"```{msg.Content}```\n\nIf you're done with the ticket, type `{config.CommandPrefix}close`, or react to the message with ☑.")
                            .WithColor(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB)
                            .WithFooter($"Time Created: {DateTime.Now}");
                        var message = await chnl.SendMessageAsync(
                            $"You can close this ticket if you have the role set for moderating tickets: `{supportConfig.SupportRole}`",
                            false, embed.Build());
                        await message.PinAsync();
                        await message.AddReactionAsync(new Emoji("☑"));
                    }
                    else {
                        var channel = context.Guild.GetTextChannel(supportChannelExists.Id);
                        await channel.SendMessageAsync(
                            $"{context.User.Mention}, please send your message here rather than the primary support channel. Text: ```{msg.Content}``` If you cannot type in here, please tell an admin.");
                        await msg.DeleteAsync();
                    }
                }
            }
        }

        public static async Task CheckMessageForEmoji(Cacheable<IUserMessage, ulong> userCache,
            ISocketMessageChannel channel, SocketReaction reaction) {
            var config = ServerConfig.Get(((SocketTextChannel) channel).Guild);
            if (reaction.Emote.Equals(new Emoji("☑"))
                && Regex.IsMatch(channel.Name, "^" + config.SupportChannelName + "-[0-9]{18}$")
                && reaction.UserId != SIVA.GetInstance.CurrentUser.Id) {
                await channel.SendMessageAsync("", false, new EmbedBuilder()
                    .WithAuthor(reaction.User.Value)
                    .WithDescription("Closing ticket in 45 seconds...")
                    .WithColor(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB).Build());
                await TicketHandler.DeleteTicket(channel);
            }
        }
    }
}