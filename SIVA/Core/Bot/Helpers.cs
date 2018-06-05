using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.Bot.Internal;
using SIVA.Core.JsonFiles;

namespace SIVA.Core.Bot
{
    internal class Helpers
    {

        /// <summary>
        ///     Creates an embed. Pass in a SocketCommandContext and a string for the embed contents.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="desc"></param>
        /// <returns></returns>

        public static EmbedBuilder CreateEmbed(SocketCommandContext ctx, string desc)
        {
            var config = GuildConfig.GetOrCreateConfig(ctx.Guild.Id);
            var embed = new EmbedBuilder()
                .WithDescription(desc)
                .WithColor(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3)
                .WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", ctx.User.Username));
            return embed;
        }

        /// <summary>
        ///     Sends a message. Pass in a SocketCommandContext, embed, and/or a string.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="embed"></param>
        /// <param name="msg"></param>
        /// <returns></returns>

        public static async Task SendMessage(SocketCommandContext ctx, EmbedBuilder embed = null, string msg = "")
        {
            if (embed == null)
                await ctx.Channel.SendMessageAsync(msg);
            else
                await ctx.Channel.SendMessageAsync(msg, false, embed);
        }

        /// <summary>
        ///     Gets a SocketTextChannel by its ID.
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>

        public static SocketTextChannel GetChannelById(ulong channelId)
        {
            return Program._client.GetChannel(channelId) as SocketTextChannel;
        }

        /// <summary>
        ///     Gets a guild by its ID.
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>

        public static SocketGuild GetGuild(ulong serverId)
        {
            return Program._client.GetGuild(serverId);
        }

        /// <summary>
        ///     Gets the bot user.
        /// </summary>
        /// <returns></returns>

        public static SocketSelfUser GetSelfUser()
        {
            return Program._client.CurrentUser;
        }

        /// <summary>
        ///     Get a user by their ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        public static SocketUser GetUserById(ulong userId)
        {
            return Program._client.GetUser(userId);
        }

        /// <summary>
        ///     Get a DM channel with a user by their ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        public static Task<IDMChannel> GetDmChannel(ulong userId)
        {
            return GetUserById(userId).GetOrCreateDMChannelAsync();
        }

        /// <summary>
        /// Get a role by its ID. 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>

        public static IEnumerable<SocketRole> GetRole(SocketCommandContext ctx, string roleName)
        {
            var role = ctx.Guild.Roles.Where(x => x.Name == roleName);
            return role;
        }


        /// <summary>
        ///     Find a user by their username and discriminator.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="userName"></param>
        /// <returns></returns>

        public static SocketUser GetUserByName(SocketCommandContext ctx, string userName)
        {
            return Program._client.GetUser(userName, userName);
        }

        /// <summary>
        ///     Checks if a user has a role, by the role's ID.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="roleId"></param>
        /// <returns>Boolean</returns>

        internal static bool UserHasRole(SocketCommandContext ctx, ulong roleId)
        {
            var targetRole = ctx.Guild.Roles.FirstOrDefault(r => r.Id == roleId);
            var gUser = ctx.User as SocketGuildUser;

            return gUser.Roles.Contains(targetRole);
        }
    }
}