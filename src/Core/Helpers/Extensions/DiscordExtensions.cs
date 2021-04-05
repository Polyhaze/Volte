using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Commands;
using Volte.Core;
using Volte.Core.Entities;
using Volte.Core.Helpers;
using Volte.Services;

namespace Gommon
{
    public static partial class Extensions
    {
        /// <summary>
        ///     Checks if the current user is the user identified in the bot's config.
        /// </summary>
        /// <param name="user">The current user</param>
        /// <returns>True, if the current user is the bot's owner; false otherwise.</returns>
        public static bool IsBotOwner(this SocketGuildUser user)
            => Config.Owner == user.Id;

        private static bool IsGuildOwner(this SocketGuildUser user)
            => user.Guild.OwnerId == user.Id || IsBotOwner(user);

        public static bool IsModerator(this SocketGuildUser user, VolteContext ctx)
        {
            return HasRole(user, ctx.GuildData.Configuration.Moderation.ModRole) ||
                   IsAdmin(user, ctx) ||
                   IsGuildOwner(user);
        }

        public static bool HasRole(this SocketGuildUser user, ulong roleId)
            => user.Roles.Select(x => x.Id).Contains(roleId);

        public static bool IsAdmin(this SocketGuildUser user, VolteContext ctx)
        {
            return HasRole(user, ctx.GuildData.Configuration.Moderation.AdminRole) ||
                   IsGuildOwner(user);
        }

        public static async Task<bool> TrySendMessageAsync(this SocketGuildUser user, string text = null,
            bool isTts = false, Embed embed = null, RequestOptions options = null)
        {
            try
            {
                await user.SendMessageAsync(text, isTts, embed, options);
                return true;
            }
            catch (HttpException)
            {
                return false;
            }
        }
        
        public static SocketRole GetHighestRole(this SocketGuildUser member)
            => member.Roles.OrderByDescending(x => x.Position).FirstOrDefault();
        
        public static SocketRole GetHighestRoleWithColor(this SocketGuildUser member) 
            => member.Roles.Where(x => x.HasColor())
                .OrderByDescending(x => x.Position).FirstOrDefault();

        public static async Task<bool> TrySendMessageAsync(this SocketTextChannel channel, string text = null,
            bool isTts = false, Embed embed = null, RequestOptions options = null)
        {
            try
            {
                await channel.SendMessageAsync(text, isTts, embed, options);
                return true;
            }
            catch (HttpException)
            {
                return false;
            }
        }

        public static string GetInviteUrl(this IDiscordClient client, bool withAdmin = true)
            => withAdmin
                ? $"https://discord.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=8"
                : $"https://discord.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=402992246";

        public static SocketUser GetOwner(this BaseSocketClient client)
            => client.GetUser(Config.Owner);

        public static SocketGuild GetPrimaryGuild(this BaseSocketClient client)
            => client.GetGuild(405806471578648588);

        public static void RegisterVolteEventHandlers(this DiscordShardedClient client, IServiceProvider provider)
        {
            var welcome = provider.Get<WelcomeService>();
            var guild = provider.Get<GuildService>();
            var evt = provider.Get<EventService>();
            var autorole = provider.Get<AutoroleService>();
            var mod = provider.Get<ModerationService>();
            
            client.Log += m =>
            {
                Logger.HandleLogEvent(new LogEventArgs(m));
                return Task.CompletedTask;
            };
            client.JoinedGuild += async g => await guild.OnJoinAsync(new JoinedGuildEventArgs(g));
            client.LeftGuild += async g => await guild.OnLeaveAsync(new LeftGuildEventArgs(g));

            client.UserJoined += async user =>
            {
                if (Config.EnabledFeatures.Welcome) await welcome.JoinAsync(new UserJoinedEventArgs(user));
                if (Config.EnabledFeatures.Autorole) await autorole.ApplyRoleAsync(new UserJoinedEventArgs(user));
                if (provider.Get<DatabaseService>().GetData(user.Guild).Configuration.Moderation.CheckAccountAge && Config.EnabledFeatures.ModLog) 
                    await mod.CheckAccountAgeAsync(new UserJoinedEventArgs(user));
            };
            client.UserLeft += async user =>
            {
                if (Config.EnabledFeatures.Welcome) await welcome.LeaveAsync(new UserLeftEventArgs(user));
            };

            client.ShardReady += async c => await evt.OnShardReadyAsync(new ShardReadyEventArgs(c, client));
            client.MessageReceived += async socketMessage =>
            {
                if (socketMessage.ShouldHandle(out var msg))
                {
                    if (msg.Channel is IDMChannel)
                        await msg.Channel.SendMessageAsync("Currently, I do not support commands via DM.");
                    else
                        await evt.HandleMessageAsync(new MessageReceivedEventArgs(socketMessage, provider));

                };
            };
        }

        public static Task<IUserMessage> SendToAsync(this EmbedBuilder e, IMessageChannel c) =>
            c.SendMessageAsync(embed: e.Build());

        public static Task<IUserMessage> SendToAsync(this Embed e, IMessageChannel c) =>
            c.SendMessageAsync(embed: e);

        public static Task<IUserMessage> ReplyToAsync(this EmbedBuilder e, IUserMessage msg) => 
            msg.ReplyAsync(embed: e.Build());

        public static Task<IUserMessage> ReplyToAsync(this Embed e, IUserMessage msg) => 
            msg.ReplyAsync(embed: e);
        

        // ReSharper disable twice UnusedMethodReturnValue.Global
        public static async Task<IUserMessage> SendToAsync(this EmbedBuilder e, IGuildUser u) =>
            await (await u.GetOrCreateDMChannelAsync()).SendMessageAsync(embed: e.Build());

        public static async Task<IUserMessage> SendToAsync(this Embed e, IGuildUser u) =>
            await (await u.GetOrCreateDMChannelAsync()).SendMessageAsync(embed: e);

        public static EmbedBuilder WithSuccessColor(this EmbedBuilder e) => e.WithColor(Config.SuccessColor);

        public static EmbedBuilder WithErrorColor(this EmbedBuilder e) => e.WithColor(Config.ErrorColor);

        public static EmbedBuilder WithRelevantColor(this EmbedBuilder e, SocketGuildUser user) =>
            e.WithColor(user.GetHighestRoleWithColor()?.Color ?? new Color(Config.SuccessColor));

        public static EmbedBuilder AppendDescription(this EmbedBuilder e, string toAppend) =>
            e.WithDescription((e.Description ?? "") + toAppend);

        public static EmbedBuilder AppendDescriptionLine(this EmbedBuilder e, string toAppend = "") =>
            e.AppendDescription($"{toAppend}\n");

        public static Emoji ToEmoji(this string str) => new Emoji(str);

        public static bool ShouldHandle(this SocketMessage message, out SocketUserMessage userMessage)
        {
            if (message is SocketUserMessage msg && !msg.Author.IsBot)
            {
                userMessage = msg;
                return true;
            }
            userMessage = null;
            return false;
        }

        public static async Task<bool> TryDeleteAsync(this IDeletable deletable, RequestOptions options = null)
        {
            try
            {
                if (deletable is null) return false;
                await deletable.DeleteAsync(options);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Task<bool> TryDeleteAsync(this IDeletable deletable, string reason)
        {
            return deletable.TryDeleteAsync(DiscordHelper.CreateRequestOptions(opts => opts.AuditLogReason = reason));
        }

        public static string GetEffectiveUsername(this SocketGuildUser user) =>
            user.Nickname ?? user.Username;

        public static bool HasAttachments(this IMessage message)
            => !message.Attachments.IsEmpty();

        public static bool HasColor(this IRole role)
            => role.Color.RawValue != 0;


        public static EmbedBuilder WithDescription(this EmbedBuilder e, StringBuilder sb) 
            => e.WithDescription(sb.ToString());

    }
}