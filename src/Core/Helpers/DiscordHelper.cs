using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Gommon;
using Volte.Commands;
using Volte.Core.Entities;
using Volte.Services;

namespace Volte.Core.Helpers
{
    public static class DiscordHelper
    {
        public static string Zws => "\u200B";
        public static string Wave => "\uD83D\uDC4B";
        public static string X => "\u274C";
        public static string BallotBoxWithCheck => "\u2611";
        public static string Clap => "\uD83D\uDC4F";
        public static string OkHand => "\uD83D\uDC4C";
        public static string One => "\u0031\u20E3";
        public static string Two => "\u0032\u20E3";
        public static string Three => "\u0033\u20E3";
        public static string Four => "\u0034\u20E3";
        public static string Five => "\u0035\u20E3";
        public static string First => "⏮";
        public static string Left => "◀";
        public static string Right => "▶";
        public static string Last => "⏭";
        public static string WhiteSquare => "⏹";
        public static string OctagonalSign => "🛑";
        public static string E1234 => "🔢";
        public static string Question => "\u2753";

        public static (Emoji One, Emoji Two, Emoji Three, Emoji Four, Emoji Five) GetPollEmojis()
            => (One.ToEmoji(), Two.ToEmoji(), Three.ToEmoji(), Four.ToEmoji(), Five.ToEmoji());

        public static (Emoji X, Emoji BallotBoxWithCheck) GetCommandEmojis()
            => (X.ToEmoji(), BallotBoxWithCheck.ToEmoji());

        public static RequestOptions CreateRequestOptions(Action<RequestOptions> initializer)
        {
            var opts = RequestOptions.Default;
            initializer(opts);
            return opts;
        }

        public static string GetUrl(this Emoji emoji)
            => $"https://i.kuro.mu/emoji/512x512/{emoji.ToString().GetUnicodePoints().Select(x => x.ToString("x2")).Join('-')}.png";


        /// <summary>
        ///     Checks if the current user is the user identified in the bot's config.
        /// </summary>
        /// <param name="user">The current user</param>
        /// <returns>True, if the current user is the bot's owner; false otherwise.</returns>
        public static bool IsBotOwner(this SocketGuildUser user)
            => Config.Owner == user.Id;

        private static bool IsGuildOwner(this SocketGuildUser user)
            => user.Guild.OwnerId == user.Id || IsBotOwner(user);

        public static bool IsModerator(this VolteContext ctx, SocketGuildUser user) 
            => user.HasRole(ctx.GuildData.Configuration.Moderation.ModRole) ||
                   ctx.IsAdmin(user) ||
                   IsGuildOwner(user);

            public static bool HasRole(this SocketGuildUser user, ulong roleId)
            => user.Roles.Select(x => x.Id).Contains(roleId);

        public static bool IsAdmin(this VolteContext ctx, SocketGuildUser user) 
            => HasRole(user, ctx.GuildData.Configuration.Moderation.AdminRole) ||
                   IsGuildOwner(user);

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

        public static bool TryGetSpotifyStatus(this IGuildUser user, out SpotifyGame spotify)
        {
            spotify = user.Activity?.Cast<SpotifyGame>() ??
                      user.Activities.FirstOrDefault(x => x is SpotifyGame).Cast<SpotifyGame>();
            return spotify != null;
        }

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
            var evt = provider.Get<EventService>();
            var autorole = provider.Get<AutoroleService>();
            var mod = provider.Get<ModerationService>();

            client.Log += m =>
            {
                Logger.HandleLogEvent(new LogEventArgs(m));
                return Task.CompletedTask;
            };
            if (provider.TryGet<GuildService>(out var guild))
            {
                client.JoinedGuild += async g => await guild.OnJoinAsync(new JoinedGuildEventArgs(g));
                client.LeftGuild += async g => await guild.OnLeaveAsync(new LeftGuildEventArgs(g));
            }


            client.UserJoined += async user =>
            {
                if (Config.EnabledFeatures.Welcome) await welcome.JoinAsync(new UserJoinedEventArgs(user));
                if (Config.EnabledFeatures.Autorole) await autorole.ApplyRoleAsync(new UserJoinedEventArgs(user));
                if (provider.Get<DatabaseService>().GetData(user.Guild).Configuration.Moderation.CheckAccountAge &&
                    Config.EnabledFeatures.ModLog)
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
                }
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
            e.WithDescription((e.Description ?? string.Empty) + toAppend);

        public static EmbedBuilder AppendDescriptionLine(this EmbedBuilder e, string toAppend = "") =>
            e.AppendDescription($"{toAppend}\n");

        /// <summary>
        ///     Removes the author and sets the color to the config-provided <see cref="Config"/>.<see cref="Config.SuccessColor"/>,
        /// however it only removes it if <see cref="ModerationOptions.ShowResponsibleModerator"/> on the provided <paramref name="data"/> is <see langword="false"/>
        /// </summary>
        /// <param name="e">The current <see cref="EmbedBuilder"/>.</param>
        /// <param name="data">The <see cref="GuildData"/> to apply settings for.</param>
        /// <returns>The possibly-modified <see cref="EmbedBuilder"/></returns>
        public static EmbedBuilder ApplyConfig(this EmbedBuilder e, GuildData data) => e.Apply(eb =>
        {
            if (!data.Configuration.Moderation.ShowResponsibleModerator) return;
            eb.WithAuthor(author: null);
            eb.WithSuccessColor();
        });

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
            => deletable.TryDeleteAsync(CreateRequestOptions(opts => opts.AuditLogReason = reason));

        public static string GetEffectiveUsername(this IGuildUser user) =>
            user.Nickname ?? user.Username;

        public static string GetEffectiveAvatarUrl(this IUser user, ImageFormat format = ImageFormat.Auto,
            ushort size = 128)
            => user.GetAvatarUrl(format, size) ?? user.GetDefaultAvatarUrl();

        public static bool HasAttachments(this IMessage message)
            => !message.Attachments.IsEmpty();

        public static bool HasColor(this IRole role)
            => role.Color.RawValue != 0;

        public static EmbedBuilder WithDescription(this EmbedBuilder e, StringBuilder sb)
            => e.WithDescription(sb.ToString());
    }
}