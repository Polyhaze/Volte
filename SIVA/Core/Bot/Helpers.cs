using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.JsonFiles;

namespace SIVA.Core.Bot 
{
    class Helpers 
    {
        public static EmbedBuilder CreateEmbed(SocketCommandContext ctx, string desc)
        {
            var config = GuildConfig.GetOrCreateConfig(ctx.Guild.Id);
            var embed = new EmbedBuilder()
                .WithDescription(desc)
                .WithColor(config.EmbedColour1, config.EmbedColour2, config.EmbedColour3)
                .WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", ctx.User.Username));
            return embed;
        }

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

        public static SocketTextChannel GetChannel(ulong serverId, ulong channelId)
        {
            return Program._client.GetGuild(serverId).GetTextChannel(channelId);
        }

        public static SocketGuild GetGuild(ulong serverId)
        {
            return Program._client.GetGuild(serverId);
        }

        public static SocketSelfUser GetSelfUser()
        {
            return Program._client.CurrentUser;
        }

        public static SocketUser GetUser(ulong userId)
        {
            return Program._client.GetUser(userId);
        }

        public static Task<IDMChannel> GetDmChannel(ulong userId)
        {
            return Program._client.GetUser(userId).GetOrCreateDMChannelAsync();
        }

        public static IEnumerable<SocketRole> GetRole(SocketCommandContext ctx, string roleName)
        {
            var role = ctx.Guild.Roles.Where(x => x.Name == roleName);
            return role;
        }
    }
}
