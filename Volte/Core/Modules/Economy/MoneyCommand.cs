using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Volte.Core.Files.Readers;
using Volte.Helpers;

namespace Volte.Core.Modules.Economy {
    public partial class EconomyModule : VolteModule {
        [Command("Money"), Alias("$", "Bal")]
        public async Task Money() {
            var userData = Db.GetUser(Context.User);
            await Context.Channel.SendMessageAsync("", false,
                CreateEmbed(Context, $"You have **${userData.Money}**"));
        }
    }
}