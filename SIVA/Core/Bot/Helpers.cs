using System.Threading.Tasks;
using Discord;
using Discord.Commands;
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
            return;
        }
    }
}
