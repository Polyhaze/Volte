using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Volte.Commands;
using Volte.Core;
using Volte.Core.Models.EventArgs;
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
        public static bool IsBotOwner(this DiscordMember user)
            => Config.Owner == user.Id;

        private static bool IsGuildOwner(this DiscordMember user)
            => user.Guild.Owner == user || IsBotOwner(user);

        public static bool IsModerator(this DiscordMember user, VolteContext ctx)
        {
            return HasRole(user, ctx.GuildData.Configuration.Moderation.ModRole) ||
                   IsAdmin(user, ctx) ||
                   IsGuildOwner(user);
        }

        private static bool HasRole(this DiscordMember user, ulong roleId)
            => user.Roles.Select(x => x.Id).Contains(roleId);

        public static bool IsAdmin(this DiscordMember user, VolteContext ctx)
        {
            return HasRole(user, ctx.GuildData.Configuration.Moderation.AdminRole) ||
                   IsGuildOwner(user);
        }

        public static DiscordRole GetHighestRole(this DiscordMember member)
        {
            var roles = member.Roles.OrderByDescending(x => x.Position);
            return roles.FirstOrDefault();
        }
        
        public static DiscordRole GetHighestRoleWithColor(this DiscordMember member)
        {
            var coloredRoles = member.Roles.Where(x => x.Color.Value != new DiscordColor(0, 0, 0).Value);
            var roles = coloredRoles.OrderByDescending(x => x.Position);
            return roles.FirstOrDefault();
        }

        public static async Task<bool> TrySendMessageAsync(this DiscordMember user, string text = null,
            bool isTts = false, DiscordEmbed embed = null)
        {
            try
            {
                await user.SendMessageAsync(text, isTts, embed);
                return true;
            }
            catch (UnauthorizedException e)
            {
                return false;
            }
        }

        public static async Task<bool> TrySendMessageAsync(this SocketTextChannel channel, string text = null,
            bool isTts = false, Embed embed = null, RequestOptions options = null)
        {
            try
            {
                await channel.SendMessageAsync(text, isTts, embed, options);
                return true;
            }
            catch (HttpException e) when (e.HttpCode is HttpStatusCode.Forbidden)
            {
                return false;
            }
        }

        public static string GetInviteUrl(this IDiscordClient client, bool withAdmin = true)
            => withAdmin
                ? $"https://discordapp.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=8"
                : $"https://discordapp.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=402992246";

        public static SocketUser GetOwner(this BaseSocketClient client)
            => client.GetUser(Config.Owner);

        public static SocketGuild GetPrimaryGuild(this BaseSocketClient client)
            => client.GetGuild(405806471578648588);
        
        // https://discord.com/developers/docs/topics/gateway#sharding-sharding-formula
        /// <summary>
        /// Gets a shard id from a guild id and total shard count.
        /// </summary>
        /// <param name="guildId">The guild id the shard is on.</param>
        /// <param name="shardCount">The total amount of shards.</param>
        /// <returns>The shard id.</returns>
        public static int GetShardId(ulong guildId, int shardCount)
            => (int)(guildId >> 22) % shardCount;

        public static DiscordGuild GetGuild(this DiscordShardedClient client, ulong guildId)
            => client.ShardClients[GetShardId(guildId, client.ShardClients.Count)].Guilds[guildId]; // TODO test

        public static void RegisterVolteEventHandlers(this DiscordShardedClient client, IServiceProvider provider)
        {
            var welcome = provider.Get<WelcomeService>();
            var guild = provider.Get<GuildService>();
            var evt = provider.Get<EventService>();
            var autorole = provider.Get<AutoroleService>();
            var logger = provider.Get<LoggingService>();
            client.Log += async m => await logger.DoAsync(new LogEventArgs(m));
            client.JoinedGuild += async g => await guild.DoAsync(new JoinedGuildEventArgs(g));
            client.LeftGuild += async g => await guild.DoAsync(new LeftGuildEventArgs(g));
            client.UserJoined += async user =>
            {
                if (Config.EnabledFeatures.Welcome) await welcome.JoinAsync(new UserJoinedEventArgs(user));
                if (Config.EnabledFeatures.Autorole) await autorole.DoAsync(new UserJoinedEventArgs(user));
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
                }
            };
        }

        public static Task<IUserMessage> SendToAsync(this EmbedBuilder e, IMessageChannel c) =>
            c.SendMessageAsync(string.Empty, false, e.Build());

        public static Task<IUserMessage> SendToAsync(this Embed e, IMessageChannel c) =>
            c.SendMessageAsync(string.Empty, false, e);

        // ReSharper disable twice UnusedMethodReturnValue.Global
        public static async Task<IUserMessage> SendToAsync(this EmbedBuilder e, IGuildUser u) =>
            await (await u.GetOrCreateDMChannelAsync()).SendMessageAsync(string.Empty, false, e.Build());

        public static async Task<IUserMessage> SendToAsync(this Embed e, IGuildUser u) =>
            await (await u.GetOrCreateDMChannelAsync()).SendMessageAsync(string.Empty, false, e);

        public static Task<DiscordMessage> SendToAsync(this DiscordEmbedBuilder builder, DiscordChannel channel) => channel.SendMessageAsync(embed: builder.Build());
        
        public static DiscordEmbedBuilder WithColor(this DiscordEmbedBuilder e, uint color) => e.WithColor(new DiscordColor((int) color));

        public static DiscordEmbedBuilder WithSuccessColor(this DiscordEmbedBuilder e) => e.WithColor(Config.SuccessColor);

        public static DiscordEmbedBuilder WithErrorColor(this DiscordEmbedBuilder e) => e.WithColor(Config.ErrorColor);

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
            return deletable.TryDeleteAsync(new RequestOptions {AuditLogReason = reason});
        }

        public static string GetEffectiveUsername(this SocketGuildUser user) =>
            user.Nickname ?? user.Username;

        public static bool HasAttachments(this IMessage message)
            => !message.Attachments.IsEmpty();

        public static bool HasColor(this IRole role)
            => !(role.Color.RawValue is 0);
    }
}