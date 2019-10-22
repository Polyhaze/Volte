using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Volte.Core;
using Volte.Core.Models.EventArgs;
using Volte.Services;

namespace Gommon
{
    public static partial class Extensions
    {
        public static bool IsBotOwner(this SocketGuildUser user)
            => Config.Owner == user.Id;

        private static bool IsGuildOwner(this SocketGuildUser user)
            => user.Guild.OwnerId == user.Id || IsBotOwner(user);

        public static bool IsModerator(this SocketGuildUser user, IServiceProvider provider)
        {
            provider.Get<DatabaseService>(out var db);
            return HasRole(user, db.GetData(user.Guild).Configuration.Moderation.ModRole) ||
                   IsAdmin(user, provider) ||
                   IsGuildOwner(user);
        }

        private static bool HasRole(this SocketGuildUser user, ulong roleId)
            => user.Roles.Select(x => x.Id).Contains(roleId);

        public static bool IsAdmin(this SocketGuildUser user, IServiceProvider provider)
        {
            provider.Get<DatabaseService>(out var db);
            return HasRole(user,
                       db.GetData(user.Guild).Configuration.Moderation.AdminRole) ||
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
            catch (HttpException e) when (e.HttpCode is HttpStatusCode.Forbidden)
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

        public static async Task<bool> TryDeleteAsync(this SocketMessage message, RequestOptions options = null)
        {
            try
            {
                await message.DeleteAsync();
                return true;
            }
            catch
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

        public static Task RegisterVolteEventHandlersAsync(this DiscordShardedClient client, IServiceProvider provider)
        {
            provider.Get<WelcomeService>(out var welcome);
            provider.Get<GuildService>(out var guild);
            provider.Get<EventService>(out var evt);
            provider.Get<AutoroleService>(out var autorole);
            provider.Get<LoggingService>(out var logger);
            return Executor.ExecuteAsync(() =>
            {
                client.Log += m => logger.DoAsync(new LogEventArgs(m));
                client.JoinedGuild += g => guild.OnJoinAsync(new JoinedGuildEventArgs(g));
                client.LeftGuild += g => guild.OnLeaveAsync(new LeftGuildEventArgs(g));
                
                client.UserJoined += async user =>
                {
                    if (Config.EnabledFeatures.Welcome)
                        await welcome.JoinAsync(new UserJoinedEventArgs(user));
                    if (Config.EnabledFeatures.Autorole)
                        await autorole.DoAsync(new UserJoinedEventArgs(user));
                };
                client.UserLeft += async user =>
                {
                    if (Config.EnabledFeatures.Welcome)
                        await welcome.LeaveAsync(new UserLeftEventArgs(user));
                };
                
                client.ShardReady += c => evt.OnReady(new ReadyEventArgs(c, client));
                client.MessageReceived += async s =>
                {
                    if (!(s is SocketUserMessage msg) || msg.Author.IsBot) return;
                    if (msg.Channel is IDMChannel dmc)
                    {
                        await dmc.SendMessageAsync("Currently, I do not support commands via DM.");
                        return;
                    }

                    await evt.HandleMessageAsync(new MessageReceivedEventArgs(s, provider));
                };
                return Task.CompletedTask;
            });
        }

        public static Task<IUserMessage> SendToAsync(this EmbedBuilder e, IMessageChannel c) =>
            c.SendMessageAsync(string.Empty, false, e.Build());

        public static Task<IUserMessage> SendToAsync(this Embed e, IMessageChannel c) =>
            c.SendMessageAsync(string.Empty, false, e);

        public static async Task<IUserMessage> SendToAsync(this EmbedBuilder e, IGuildUser u) =>
            await (await u.GetOrCreateDMChannelAsync()).SendMessageAsync(string.Empty, false, e.Build());

        public static async Task<IUserMessage> SendToAsync(this Embed e, IGuildUser u) =>
            await (await u.GetOrCreateDMChannelAsync()).SendMessageAsync(string.Empty, false, e);

        public static EmbedBuilder WithSuccessColor(this EmbedBuilder e) => e.WithColor(Config.SuccessColor);

        public static EmbedBuilder WithErrorColor(this EmbedBuilder e) => e.WithColor(Config.ErrorColor);

        public static Emoji ToEmoji(this string str) => new Emoji(str);
    }
}