using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Data;

namespace Volte.Core.Modules.Economy {
    public partial class EconomyModule : VolteModule {
        [Command("Pay")]
        [Summary("Pays the given user x amount of money.")]
        [Remarks("Usage: |prefix|pay {@user} {amount}")]
        public async Task Pay(SocketGuildUser user, int moneyToPay) {
            var embed = CreateEmbed(Context, string.Empty)
                .ToEmbedBuilder()
                .WithAuthor(Context.User);
            var ua = Db.GetUser(Context.User);
            var ua1 = Db.GetUser(user);

            if (ua.Money < moneyToPay) {
                embed.WithDescription($"You don't have enough money, {Context.User.Mention}!");
                await Context.Channel.SendMessageAsync(string.Empty, false, embed.Build());
            }
            else {
                ua.Money -= moneyToPay;
                ua1.Money += moneyToPay;
                Db.UpdateUser(ua);
                Db.UpdateUser(ua1);
                embed.WithDescription($"{Context.User.Mention} paid {user.Mention} ${moneyToPay}!");
                await Context.Channel.SendMessageAsync(string.Empty, false, embed.Build());
            }
        }
    }
}