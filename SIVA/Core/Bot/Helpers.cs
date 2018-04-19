using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.JsonFiles;

namespace SIVA.Core.Bot 
{
    class Helpers 
    {
        
        /*
         * <summary>
         * 
         *     Creates an embed and returns the finished EmbedBuilder value for easy embed creation.
         * 
         * </summary>
         */
        public static EmbedBuilder CreateEmbed(SocketCommandContext ctx, string desc)
        {
            var config = GuildConfig.GetOrCreateConfig(ctx.Guild.Id);
            var embed = new EmbedBuilder()
                .WithDescription(desc)
                .WithColor(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3)
                .WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", ctx.User.Username));
            return embed;
        }
        
        /*
         * <summary>
         *
         *     Sends a message in the channel context.
         *
         * </summary>
         */

        public static async Task SendMessage(SocketCommandContext ctx, EmbedBuilder embed = null, string msg = "")
        {
            if (embed == null)
            {
                await ctx.Channel.SendMessageAsync(msg);
            }
            else
            {
                await ctx.Channel.SendMessageAsync(msg, false, embed);
            }
        }
        
        /*
         * <summary>
         *
         *     Gets a channel based on the Server ID and Channel ID given.
         * 
         * </summary>
         */

        public static SocketTextChannel GetChannelById(ulong channelId)
        {
            return Program._client.GetChannel(channelId) as SocketTextChannel;
        }
        
        /*
         * <summary>
         *
         *     Gets a guild based on the Server ID given.
         * 
         * </summary>
         */

        public static SocketGuild GetGuild(ulong serverId)
        {
            return Program._client.GetGuild(serverId);
        }
        
        /*
         * <summary>
         *
         *     Get the bot user.
         * 
         * </summary>
         */

        public static SocketSelfUser GetSelfUser()
        {
            return Program._client.CurrentUser;
        }
        
        /*
         * <summary>
         *
         *     Gets a user based on their ID.
         * 
         * </summary>
         */

        public static SocketUser GetUserById(ulong userId)
        {
            return Program._client.GetUser(userId);
        }
        
        /*
         * <summary>
         *
         *     Get a DM channel with a user by their ID.
         * 
         * </summary>
         */

        public static Task<IDMChannel> GetDmChannel(ulong userId)
        {
            return GetUserById(userId).GetOrCreateDMChannelAsync();
        }
        
        /*
         * <summary>
         *
         *     Get a role based on its name with LINQ.
         * 
         * </summary>
         */

        public static IEnumerable<SocketRole> GetRole(SocketCommandContext ctx, string roleName)
        {
            var role = ctx.Guild.Roles.Where(x => x.Name == roleName);
            return role;
        }

        public static SocketUser GetUserByName(SocketCommandContext ctx, string userName)
        {
            return Program._client.GetUser(userName, userName);
        }
    }
}
