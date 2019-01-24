using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.Economy {
    public class MoneyCommand : VolteCommand {
        [Command("Money"), Alias("$", "Bal")]
        public async Task Money() {
            var userData = Users.Get(Context.User.Id);
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"You have **${userData.Money}**"));
        }
    }
}