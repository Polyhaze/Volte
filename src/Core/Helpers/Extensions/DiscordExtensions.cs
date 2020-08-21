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
using DSharpPlus.Interactivity;
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

        public static async Task<bool> TrySendMessageAsync(this DiscordChannel channel, string text = null,
            bool isTts = false, DiscordEmbed embed = null)
        {
            try
            {
                await channel.SendMessageAsync(text, isTts, embed);
                return true;
            }
            catch (UnauthorizedException)
            {
                return false;
            }
        }

        public static string GetInviteUrl(this DiscordShardedClient client, bool withAdmin = true)
            => withAdmin
                ? $"https://discordapp.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=8"
                : $"https://discordapp.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=402992246";

        public static DiscordGuild GetPrimaryGuild(this DiscordShardedClient client)
            => client.GetGuild(405806471578648588);

        public static DiscordEmbedBuilder AddField(this DiscordEmbedBuilder builder, string name, object value,
            bool inline = false)
            => builder.AddField(name, value.ToString(), inline);
        
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
            client.DebugLogger.LogMessageReceived += async (_, args) => await logger.DoAsync(new LogEventArgs(args));
            client.GuildCreated += async args => await guild.DoAsync(args);
            client.GuildDeleted += async args => await guild.DoAsync(args);
            client.GuildMemberAdded += async args =>
            {
                if (Config.EnabledFeatures.Welcome) await welcome.JoinAsync(args);
                if (Config.EnabledFeatures.Autorole) await autorole.DoAsync(args);
            };
            client.GuildMemberRemoved += async args =>
            {
                if (Config.EnabledFeatures.Welcome) await welcome.LeaveAsync(args);
            };

            client.Ready += async args => await evt.OnShardReadyAsync(args);
            client.MessageCreated += async args =>
            {
                if (args.Message.ShouldHandle())
                {
                    if (args.Channel is DiscordDmChannel)
                        await args.Channel.SendMessageAsync("I do not support commands via DM.");
                    else
                        await evt.HandleMessageAsync(new MessageReceivedEventArgs(args.Message, provider));
                }
            };
        }

        public static Task<DiscordMessage> SendToAsync(this DiscordEmbedBuilder e, DiscordChannel c) =>
            c.SendMessageAsync(string.Empty, false, e.Build());

        public static Task<DiscordMessage> SendToAsync(this DiscordEmbed e, DiscordChannel c) =>
            c.SendMessageAsync(string.Empty, false, e);

        // ReSharper disable twice UnusedMethodReturnValue.Global
        public static async Task<DiscordMessage> SendToAsync(this DiscordEmbedBuilder e, DiscordMember u) =>
            await u.SendMessageAsync(string.Empty, false, e.Build());

        public static async Task<DiscordMessage> SendToAsync(this DiscordEmbed e, DiscordMember u) =>
            await u.SendMessageAsync(string.Empty, false, e);

        public static DiscordEmbedBuilder WithColor(this DiscordEmbedBuilder e, uint color) => e.WithColor(new DiscordColor((int) color));

        public static DiscordEmbedBuilder WithSuccessColor(this DiscordEmbedBuilder e) => e.WithColor(Config.SuccessColor);

        public static DiscordEmbedBuilder WithErrorColor(this DiscordEmbedBuilder e) => e.WithColor(Config.ErrorColor);

        public static DiscordEmoji ToEmoji(this string str) => DiscordEmoji.FromUnicode(str);

        public static bool ShouldHandle(this DiscordMessage message)
        {
            return !message.Author.IsBot;
        }

        public static string GetEffectiveUsername(this DiscordMember user) =>
            user.Nickname ?? user.Username;

        public static bool HasAttachments(this DiscordMessage message)
            => !message.Attachments.IsEmpty();

        public static bool HasColor(this DiscordRole role)
            => !(role.Color.Value is 0);
    }
}