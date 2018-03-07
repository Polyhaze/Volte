using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace SIVA.Core.Modules //having this anywhere else would be pointless
{
    /* This class is for shortening the actions the bot does, such as Sending a message.
    Will make this more useful in the future */
    public class BotUtils : ModuleBase<SocketCommandContext>
    {
        public async Task Reply(Embed embed, string text = "", bool isTTS = false)
        {
            await ReplyAsync(text, isTTS, embed);
        }

        public async Task Embed(string desc)
        {
            var embed = new EmbedBuilder();
            embed.WithDescription(desc);
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            
            await ReplyAsync("", false, embed);

        }
    }
}
