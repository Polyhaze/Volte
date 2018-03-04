using Discord.Commands;
using System.Threading.Tasks;
using SIVA.Core.UserAccounts;
using Discord;
using Discord.WebSocket;

namespace SIVA.Core.Modules
{
    public class Economy : ModuleBase<SocketCommandContext>
    {
        [Command("Money"), Alias("$", "bal")]
        public async Task HowMuchDoIHave()
        {
            var ua = UserAccounts.UserAccounts.GetAccount(Context.User);
            var bal = ua.Money.ToString();
            var embed = new EmbedBuilder();

            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            embed.WithDescription(Utilities.GetFormattedLocaleMsg("MoneyCommandText", bal));
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithThumbnailUrl("http://www.stickpng.com/assets/images/580b585b2edbce24c47b2878.png");

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("Pay")]
        public async Task PayAUser(SocketGuildUser user, int amt)
        {
            var embed = new EmbedBuilder();
            embed.WithColor(SIVA.Config.bot.DefaultEmbedColour);
            embed.WithFooter(Utilities.GetFormattedLocaleMsg("CommandFooter", Context.User.Username));
            var ua = UserAccounts.UserAccounts.GetAccount(Context.User);
            var ua1 = UserAccounts.UserAccounts.GetAccount(user);
            if (ua.Money < amt)
            {
                embed.WithDescription($"You don't have enough money, {Context.User.Mention}!");
                await Context.Channel.SendMessageAsync("", false, embed);
            }
            else
            {
                ua.Money = ua.Money - amt;
                ua1.Money = ua1.Money + amt;
                UserAccounts.UserAccounts.SaveAccounts();
                embed.WithDescription($"{Context.User.Mention} paid {user.Mention} {SIVA.Config.bot.CurrencySymbol}{amt}!");
                await Context.Channel.SendMessageAsync("", false, embed);
            }
        }
    }
}
