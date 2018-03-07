using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SIVA.Core.UserAccounts;

namespace SIVA.Core.Modules
{
    public class Leveling : ModuleBase<SocketCommandContext>
    {
        [Command("WhatLevelIs"), Alias("WLI")]
        public async Task WhatLevelIs(uint xp)
        {
            uint level = (uint)Math.Sqrt(xp / 50);
            await ReplyAsync("The level is " + level);
        }

        [Command("Level"), Priority(0)]
        public async Task Level()
        {
            var ua = UserAccounts.UserAccounts.GetAccount(Context.User);
            var embed = new EmbedBuilder();
            embed.WithTitle("User Level");
            embed.WithDescription(Utilities.GetFormattedLocaleMsg("LevelCommandText", Context.User.Mention, ua.LevelNumber));
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);

            await ReplyAsync("", false, embed);
        }

        [Command("Level"), Priority(1)]
        public async Task Level(SocketGuildUser user)
        {
            var ua = UserAccounts.UserAccounts.GetAccount(user);
            var embed = new EmbedBuilder();
            embed.WithTitle("User Level");
            embed.WithDescription(Utilities.GetFormattedLocaleMsg("LevelCommandText", user.Mention, ua.LevelNumber));
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);

            await ReplyAsync("", false, embed);
        }
    }
}
