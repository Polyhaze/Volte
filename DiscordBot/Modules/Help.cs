using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace SIVA.Modules
{
    public class Help : ModuleBase<SocketCommandContext>
    {

        [Command("Help"), Alias("H"), Priority(1)]
        public async Task HelpCommandWithoutArgs()
        {
            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetLocaleMsg("HelpCommandNoArgs"));
            embed.WithColor(Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithThumbnailUrl("http://www.clker.com/cliparts/b/F/3/q/f/M/help-browser-hi.png");

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("Help"), Alias("H"), Priority(0)]
        public async Task HelpCommandWithArgs(string module)
        {
            var embed = new EmbedBuilder();
            embed.WithColor(Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithThumbnailUrl("http://www.clker.com/cliparts/b/F/3/q/f/M/help-browser-hi.png");

            switch (module)
            {
                case "Economy":
                case "economy":
                    embed.WithDescription(Utilities.GetLocaleMsg("EconomyCmdList"));
                    await Context.Channel.SendMessageAsync("", false, embed);
                    break;
                case "General":
                case "general":
                    embed.WithDescription(Utilities.GetLocaleMsg("GeneralCmdList"));
                    await Context.Channel.SendMessageAsync("", false, embed);
                    break;
                case "Leveling":
                case "leveling":
                    embed.WithDescription(Utilities.GetLocaleMsg("LevelingCmdList"));
                    await Context.Channel.SendMessageAsync("", false, embed);
                    break;
                case "Moderation":
                case "moderation":
                    embed.WithDescription(Utilities.GetLocaleMsg("ModerationCmdList"));
                    await Context.Channel.SendMessageAsync("", false, embed);
                    break;
                case "Utils":
                case "utils":
                    embed.WithDescription(Utilities.GetLocaleMsg("UtilsCmdList"));
                    await Context.Channel.SendMessageAsync("", false, embed);
                    break;
                case "Stats":
                case "stats":
                    embed.WithDescription(Utilities.GetLocaleMsg("StatsCmdList"));
                    await Context.Channel.SendMessageAsync("", false, embed);
                    break;
                case "Support":
                case "support":
                    embed.WithDescription(Utilities.GetLocaleMsg("SupportCmdList"));
                    await Context.Channel.SendMessageAsync("", false, embed);
                    break;
            }
        }
    }
}
