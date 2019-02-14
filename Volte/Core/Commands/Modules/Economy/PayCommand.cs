using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Economy
{
    public partial class EconomyModule : VolteModule
    {
        [Command("Pay")]
        [Description("Pays the given user x amount of money.")]
        [Remarks("Usage: |prefix|pay {@user} {amount}")]
        public async Task Pay(SocketGuildUser user, int moneyToPay)
        {
            var embed = Context.CreateEmbed(string.Empty)
                .ToEmbedBuilder();
            var ua = Db.GetUser(Context.User);
            var ua1 = Db.GetUser(user);

            if (ua.Money < moneyToPay)
            {
                embed.WithDescription($"You don't have enough money, {Context.User.Mention}!");
                await embed.SendTo(Context.Channel);
            }
            else
            {
                ua.Money -= moneyToPay;
                ua1.Money += moneyToPay;
                Db.UpdateUser(ua);
                Db.UpdateUser(ua1);
                embed.WithDescription($"{Context.User.Mention} paid {user.Mention} ${moneyToPay}!");
                await embed.SendTo(Context.Channel);
            }
        }
    }
}