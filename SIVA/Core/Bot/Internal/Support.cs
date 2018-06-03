using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.JsonFiles;

namespace SIVA.Core.Bot.Internal
{
    internal static class Support
    {
        public static async Task SupportSystem(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            var context = new SocketCommandContext(new DiscordSocketClient(), msg);
            if (context.User.IsBot) return;
            var user = context.User as SocketGuildUser;

            var config = GuildConfig.GetGuildConfig(context.Guild.Id) ??
                         GuildConfig.CreateGuildConfig(context.Guild.Id);
            config.GuildOwnerId = context.Guild.OwnerId;
            var adminRole = context.Guild.Roles.FirstOrDefault(x => x.Id == config.AdminRole);

            if (msg.Content == "SetupSupport" && user.Roles.Contains(adminRole))
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
                var supportStartChannel =
                    context.Guild.Channels.FirstOrDefault(c => c.Name == supportConfig.SupportChannelName);

                if (msg.Channel == supportStartChannel)
                {
                    var supportChannelExists = context.Guild.Channels.FirstOrDefault(c =>
                        c.Name == $"{supportConfig.SupportChannelName}-{context.User.Id}");
                    var role = context.Guild.Roles.FirstOrDefault(r => r.Name == supportConfig.SupportRole);

                    if (supportChannelExists == null)
                    {
                        await msg.DeleteAsync();
                        var chnl = await context.Guild.CreateTextChannelAsync(
                            $"{supportConfig.SupportChannelName}-{context.User.Id}");
                        await chnl.AddPermissionOverwriteAsync(context.User,
                            new OverwritePermissions(readMessages: PermValue.Allow, sendMessages: PermValue.Allow,
                                addReactions: PermValue.Allow, sendTTSMessages: PermValue.Deny));
                        await chnl.AddPermissionOverwriteAsync(context.Guild.EveryoneRole,
                            new OverwritePermissions(readMessages: PermValue.Deny, sendMessages: PermValue.Deny));
                        if (role != null)
                            await chnl.AddPermissionOverwriteAsync(role, OverwritePermissions.AllowAll(chnl));

                        await chnl.ModifyAsync(x =>
                        {
                            x.Position = supportStartChannel.Position - 1;
                            x.Topic = $"Support ticket created by <@{msg.Author.Id}> at {DateTime.UtcNow} (UTC)";
                        });
                        var embed = new EmbedBuilder()
                            .WithAuthor(msg.Author)
                            .WithThumbnailUrl(context.User.GetAvatarUrl())
                            .WithTitle("What do you need help with?")
                            .WithDescription(
                                $"```{msg.Content}```\n\nIf you're done with the ticket, type `{config.CommandPrefix}close`, or react to the message with ☑.")
                            .WithColor(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3)
                            .WithFooter($"Time Created: {DateTime.Now}");
                        var message = await chnl.SendMessageAsync(
                            $"You can close this ticket if you have the role set for moderating tickets: `{supportConfig.SupportRole}`",
                            false, embed);
                        await message.PinAsync();
                        await message.AddReactionAsync(new Emoji("☑"));
                    }
                    else
                    {
                        var channel = context.Guild.GetTextChannel(supportChannelExists.Id);
                        await channel.SendMessageAsync(
                            $"{context.User.Mention}, please send your message here rather than the primary support channel. Text: ```{msg.Content}``` If you cannot type in here, please tell an admin.");
                        await msg.DeleteAsync();
                    }
                }
            }
        }

        public static async Task DeleteSupportChannel(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel ch,
            SocketReaction s)
        {
            var channel = s.Channel as SocketGuildChannel; //allow us to send a message
            var config =
                GuildConfig.GetGuildConfig(channel.Guild
                    .Id); //get the config so we can see the support channel name. (and embed colour)
            var embed = new EmbedBuilder() //create the embedded message.
                .WithColor(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3)
                .WithDescription($"Ticket marked as solved by {s.User.Value.Mention}! Closing in 45 seconds.")
                .WithAuthor(s.User.Value);
            if (channel.Name.Contains($"{config.SupportChannelName}-{s.UserId}") && s.Emote.Equals(new Emoji("☑"))
                ) //check if a user made the reaction in a support ticket, 
                //check if the emote is `☑` and then delete the channel.
                if (s.UserId != 320942091049893888 && s.UserId != 410547925597421571
                ) //check if the id of the person who made the reaction is NOT SIVA-dev or SIVA public.
                {
                    await ch.SendMessageAsync("", false, embed); //send the embedded message defined above.
                    Thread.Sleep(45000); //wait 45 seconds (45000ms)
                    await channel.DeleteAsync(); //delete the channel
                }
        }
    }
}