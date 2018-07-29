using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Files.Objects;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Discord.Support {
    public class SupportMessageListener {
        public static async Task Check(SocketMessage s) {
            var msg = (SocketUserMessage) s;
            var ctx = new SocketCommandContext(SIVA.GetInstance(), msg);
            var config = ServerConfig.Get(ctx.Guild);

            if (msg.Content.ToLower() == "setupsupport" && msg.Author.Id != SIVA.GetInstance().CurrentUser.Id) {
                if (!UserUtils.IsAdmin(ctx)) {
                    await ctx.Message.AddReactionAsync(new Emoji(RawEmoji.X));
                    return;
                }

                config.SupportChannelName = ctx.Channel.Name;
                config.SupportChannelId = ctx.Channel.Id;
                config.SupportCategoryId =
                    ((INestedChannel) ctx.Channel)
                    .CategoryId; //no way I can get the category, automatically, without casting. kinda annoying
                ServerConfig.Save();

                await ctx.Channel.SendMessageAsync("", false,
                    Utils.CreateEmbed(ctx,
                        "To create a support ticket, send a message into this channel. Support tickets will be placed under the " +
                        $"**{SIVA.GetInstance().GetGuild(ctx.Guild.Id).GetTextChannel(ctx.Channel.Id).Category.Name}** " +
                        "channel category.")).ConfigureAwait(false);
                return;
            }

            if (ctx.Channel.Name.Equals(config.SupportChannelName) &&
                msg.Author.Id != SIVA.GetInstance().CurrentUser.Id) {
                await CreateSupportChannel(ctx, config);
                await msg.DeleteAsync();
            }
        }

        public static async Task CreateSupportChannel(SocketCommandContext ctx, Server config) {
            var supportRole = ctx.Guild.Roles.FirstOrDefault(r => r.Name.ToLower() == config.SupportRole.ToLower());
            var channel = await ctx.Guild.CreateTextChannelAsync($"{config.SupportChannelName}-{ctx.User.Id}");
            if (supportRole == null) {
                await SIVA.GetInstance().GetUser(ctx.Guild.OwnerId).GetOrCreateDMChannelAsync().GetAwaiter().GetResult()
                    .SendMessageAsync("", false,
                        Utils.CreateEmbed(ctx,
                            "**Hey there!**\n\n" +
                            "Your support system configuration is messed up. " +
                            "The role you set to manage Support Tickets doesn't exist. " +
                            $"To fix this, either create another role named **{config.SupportRole}**, " +
                            $"or run the command `{config.CommandPrefix}supportrole RoleNameHere`. " +
                            "Until you do this, only the ticket creator and admins can see the ticket."));
            }
            else {
                await channel.AddPermissionOverwriteAsync(
                    supportRole,
                    new
                        OverwritePermissions(
                            viewChannel: PermValue.Allow,
                            sendMessages: PermValue.Allow,
                            addReactions: PermValue.Allow,
                            sendTTSMessages: PermValue.Deny
                        )
                );
            }

            await channel.ModifyAsync(x => x.CategoryId = config.SupportCategoryId);

            await channel.AddPermissionOverwriteAsync(
                ctx.User,
                new
                    OverwritePermissions(
                        viewChannel: PermValue.Allow,
                        sendMessages: PermValue.Allow,
                        addReactions: PermValue.Allow,
                        sendTTSMessages: PermValue.Deny
                    )
            );
            await channel.AddPermissionOverwriteAsync(
                ctx.Guild.EveryoneRole,
                new
                    OverwritePermissions(
                        viewChannel: PermValue.Deny,
                        sendMessages: PermValue.Deny
                    )
            );

            await TicketHandler.OnTicketCreation(ctx, channel, config);
        }
    }
}