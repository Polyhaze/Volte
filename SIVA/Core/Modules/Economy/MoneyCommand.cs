using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using SIVA.Core.Files.Readers;
using SIVA.Helpers;

namespace SIVA.Core.Modules.Economy {
    public class MoneyCommand : SIVACommand {
        [Command("Money"), Alias("$", "Bal")]
        public async Task Money() {
            var userData = Users.Get(Context.User.Id);
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"You have **${userData.Money}**"));
        }
    }
}