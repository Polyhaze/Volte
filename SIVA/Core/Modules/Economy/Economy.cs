using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System;
using SIVA.Core.Bot.Internal;
using SIVA.Core.Bot;
using SIVA.Core.JsonFiles;

namespace SIVA.Core.Modules.Economy
{
    public class Economy : ModuleBase<SocketCommandContext>
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
            var ua = UserAccounts.GetAccount(Context.User);
            var embed = Helpers.CreateEmbed(Context, Bot.Internal.Utilities.GetFormattedLocaleMsg("LevelCommandText", Context.User.Mention, ua.LevelNumber));
            embed.WithTitle("User Level");

            await Helpers.SendMessage(Context, embed);
        }

        [Command("Level"), Priority(1)]
        public async Task Level(SocketGuildUser user)
        {
            var ua = UserAccounts.GetAccount(user);
            var embed = Helpers.CreateEmbed(Context, Bot.Internal.Utilities.GetFormattedLocaleMsg("LevelCommandText", user.Mention, ua.LevelNumber));
            embed.WithTitle("User Level");

            await Helpers.SendMessage(Context, embed);
        }

        [Command("Money"), Alias("$", "bal")]
        public async Task HowMuchDoIHave()
        {
            var ua = UserAccounts.GetAccount(Context.User);
            var bal = ua.Money.ToString();
            var embed = Helpers.CreateEmbed(Context, Bot.Internal.Utilities.GetFormattedLocaleMsg("MoneyCommandText", bal));
            embed.WithThumbnailUrl("http://www.stickpng.com/assets/images/580b585b2edbce24c47b2878.png");

            await Helpers.SendMessage(Context, embed);
        }

        [Command("Pay")]
        public async Task PayAUser(SocketGuildUser user, int amt)
        {
            var embed = new EmbedBuilder();
            embed.WithColor(Config.bot.DefaultEmbedColour);
            embed.WithFooter(Bot.Internal.Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            var ua = UserAccounts.GetAccount(Context.User);
            var ua1 = UserAccounts.GetAccount(user);
            if (ua.Money < amt)
            {
                embed.WithDescription($"You don't have enough money, {Context.User.Mention}!");
                await Helpers.SendMessage(Context, embed);
            }
            else
            {
                ua.Money = ua.Money - amt;
                ua1.Money = ua1.Money + amt;
                UserAccounts.SaveAccounts();
                embed.WithDescription($"{Context.User.Mention} paid {user.Mention} {Config.bot.CurrencySymbol}{amt}!");
                await Helpers.SendMessage(Context, embed);
            }
        }
    }
}
