using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class Help : ModuleBase<SocketCommandContext>
    {

        [Command("Help"), Alias("H"), Priority(1)]
        public async Task HelpCommandWithoutArgs()
        {
            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetAlert("HelpCommandNoArgs"));
            embed.WithColor(Config.bot.defaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithThumbnailUrl("http://www.clker.com/cliparts/b/F/3/q/f/M/help-browser-hi.png");

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("Help"), Alias("H"), Priority(0)]
        public async Task HelpCommandWithArgs(string module)
        {
            var embed = new EmbedBuilder();
            embed.WithColor(Config.bot.defaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithThumbnailUrl("http://www.clker.com/cliparts/b/F/3/q/f/M/help-browser-hi.png");

            if (module == "Economy" || module == "economy")
            {
                embed.WithDescription(Utilities.GetAlert("EconomyCmdList"));
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            if (module == "General" || module == "general")
            {
                embed.WithDescription(Utilities.GetAlert("GeneralCmdList"));
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            if (module == "Leveling" || module == "leveling")
            {
                embed.WithDescription(Utilities.GetAlert("LevelingCmdList"));
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            if (module == "Moderation" || module == "moderation")
            {
                embed.WithDescription(Utilities.GetAlert("ModerationCmdList"));
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            if (module == "Utils" || module == "utils")
            {
                embed.WithDescription(Utilities.GetAlert("UtilsCmdList"));
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            if (module == "Stats" || module == "stats")
            {
                embed.WithDescription(Utilities.GetAlert("StatsCmdList"));
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            if (module == "Support" || module == "support")
            {
                embed.WithDescription(Utilities.GetAlert("SupportCmdList"));
                await Context.Channel.SendMessageAsync("", false, embed);
            }
        }
    }
}
