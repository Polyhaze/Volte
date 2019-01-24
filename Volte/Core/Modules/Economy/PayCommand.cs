using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Volte.Core.Files.Readers;

namespace Volte.Core.Modules.Economy {
    public class PayCommand : VolteCommand {
        [Command("Pay")]
        public async Task Pay(SocketGuildUser user, int moneyToPay) {
            var config = ServerConfig.Get(Context.Guild);
            var embed = new EmbedBuilder()
                .WithColor(new Color(config.EmbedColourR, config.EmbedColourG, config.EmbedColourB))
                .WithAuthor(Context.User);
            var ua = Users.Get(Context.User.Id);
            var ua1 = Users.Get(user.Id);

            if (ua.Money < moneyToPay) {
                embed.WithDescription($"You don't have enough money, {Context.User.Mention}!");
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            else {
                ua.Money -= moneyToPay;
                ua1.Money += moneyToPay;
                Users.Save();
                embed.WithDescription($"{Context.User.Mention} paid {user.Mention} ${moneyToPay}!");
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }
    }
}