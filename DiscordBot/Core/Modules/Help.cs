using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using SIVA.Core.Config;
using SIVA.Core.Bot;

namespace SIVA.Core.Modules
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command("Help"), Alias("H")]
        [Summary("Shows commands for the bot.")]
        public async Task HelpCommand(string module = "")
        {
            var config = GuildConfig.GetGuildConfig(Context.Guild.Id) ?? GuildConfig.CreateGuildConfig(Context.Guild.Id);
            var embed = new EmbedBuilder();
            embed.WithColor(Bot.Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithThumbnailUrl("http://www.clker.com/cliparts/b/F/3/q/f/M/help-browser-hi.png");

            switch (module)
            {
                case "Economy":
                case "economy":
                    embed.WithDescription(Utilities.GetLocaleMsg("EconomyCmdList"));
                    await ReplyAsync("", false, embed);
                    break;
                case "General":
                case "general":
                    embed.WithDescription(Utilities.GetLocaleMsg("GeneralCmdList"));
                    await ReplyAsync("", false, embed);
                    break;
                case "Moderation":
                case "moderation":
                    embed.WithDescription(Utilities.GetLocaleMsg("ModerationCmdList"));
                    await ReplyAsync("", false, embed);
                    break;
                case "Utils":
                case "utils":
                    embed.WithDescription(Utilities.GetLocaleMsg("UtilsCmdList"));
                    await ReplyAsync("", false, embed);
                    break;
                case "Stats":
                case "stats":
                    embed.WithDescription(Utilities.GetLocaleMsg("StatsCmdList"));
                    await ReplyAsync("", false, embed);
                    break;
                case "Admin":
                case "admin":
                    embed.WithDescription(Utilities.GetLocaleMsg("AdminCmdList"));
                    await ReplyAsync("", false, embed);
                    break;
                default:
                    embed.WithDescription(Utilities.GetFormattedLocaleMsg("HelpCommandNoArgs", config.CommandPrefix));
                    await ReplyAsync("", false, embed);
                    break;
            }
        }
    }
}
