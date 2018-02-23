using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot;

namespace DiscordBot.Modules
{
    public class Help : ModuleBase<SocketCommandContext>
    {

        [Command("help"), Alias("h"), Priority(1)]
        public async Task HelpCommandWithoutArgs()
        {
            var embed = new EmbedBuilder();
            embed.WithDescription(Utilities.GetAlert("HelpCommandNoArgs"));
            embed.WithColor(Config.bot.defaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithThumbnailUrl("http://www.clker.com/cliparts/b/F/3/q/f/M/help-browser-hi.png");

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("help"), Alias("h"), Priority(0)]
        public async Task HelpCommandWithArgs(string module)
        {
            var embed = new EmbedBuilder();
            embed.WithColor(Config.bot.defaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedAlert("CommandFooter", Context.User.Username));
            embed.WithThumbnailUrl("http://www.clker.com/cliparts/b/F/3/q/f/M/help-browser-hi.png");

            if (module == "Economy" || module == "economy")
            {
                embed.WithDescription("**Economy Module**:\n\nMoney - Aliases: $, bal\n- Shows how much money you have.");
            }
            if (module == "General" || module == "general")
            {
                embed.WithDescription("**General Module**:\n\nStats\n- Shows the mentioned users' account XP and Points.\n\nSay\n- Bot repeats what you tell it to.\n\nChoose\n- Put anything after the command, with a | inbetween, and it will choose something for you.\n\nRoast - INCOMPLETE\n- Roasts the command sender.\n\nInfo\n- Shows stats about the bot and various tidbits of info.\n\nSuggest\n- Suggest a command or something via a Google Form.");
            }
            if (module == "Leveling" || module == "leveling")
            {
                embed.WithDescription("**Leveling Module**:\n\nWhatLevelIs - Aliases: wli\n- Shows what level is equal to the given XP amount.\n\nLevel\n- Shows the level of yourself or a mentioned user.");
            }
            if (module == "Moderation" || module == "moderation")
            {
                embed.WithDescription("**Moderation Module**:\n\nBan\n- Bans a mentioned user.\n\nIdBan\n- Bans a user by their User ID.\n\nKick\n- Kicks a user.\n\nAddRole - Aliases: AR\n- Gives a role to a certain user.\n`ar @SomeUser role`\n\nRemRole - Aliases: RR\n- Removes a role from a certain user.\n`rr @SomeUser role`\n\nAutoRole\n- Sets the role to automatically be given to a user. Requires Admin permission.\n\nPurge\n- Deletes last X amount of messages.\n\nWarn\n- Warn the mentioned user with a reason afterwards.\n\nWarns\n- Gets your warns or a mentioned user's warns.");
            }
            if (module == "Utils" || module == "utils")
            {
                embed.WithDescription("**Utils Module**:\n\nYouTube - Aliases: Yt\n- Searches YouTube with the specified text.\n\nUserInfo - Aliases: useri, ui, uinfo\n- Shows information about a user or yourself.\n\nCalculator - Aliases: Calc\n- Syntax: `calc operator firstNumber secondNumber`\nValid operators are `sub`, `add`, `div`, and `mult`.\n\nServerInfo - Aliases: serveri, sinfo, si\n- Shows information about the server the command was sent in.\n\nPing\n- Pings the bot, lol.\n\nGoogle\n- Searches whatever you specify after the command.\n\nAddXp\n Gives specified user an XP amount, breaks leveling. Requires admin permission.\n\nInvite\n- Get the invite for the bot.");
            }
            if (module == "Stats" || module == "stats")
            {
                embed.WithDescription("**Stats Module**:\n\nOverwatchPlayer - Aliases: owp\n- Shows stats (albeit limited) about a specified user.\n\nFortnite - Aliases: fn\n- Shows stats (super limited lol) for Fortnite.");
            }
            if (module == "Support" || module == "support")
            {
                embed.WithDescription("**Support Module**:\n\nSupportCloseTicket - Aliases: SCT, Close\n- Closes your ticket. If you don't have one, it doesn't delete it. If you mention a user it deletes their ticket. (Requires admin permission)\n\nSupportChannelName - Aliases: SCN\n- Sets the name of the channel for primary support. E.G. support\n\nSupportCloseOwnTicket - Aliases: SCOT\n- Sets if a user can close their own ticket to True or False.\n\nAll of these commands, except SupportCloseTicket, need the Administrator permission to run.");
            }

            await Context.Channel.SendMessageAsync("", false, embed);
        }
    }
}
