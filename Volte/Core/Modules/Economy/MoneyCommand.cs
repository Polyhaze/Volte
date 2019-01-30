using System.Threading.Tasks;
using Discord.Commands;

namespace Volte.Core.Modules.Economy {
    public partial class EconomyModule : VolteModule {
        [Command("Money"), Alias("$", "Bal")]
        [Summary("Checks how much money you have.")]
        [Remarks("Usage: |prefix|money")]
        public async Task Money() {
            var userData = Db.GetUser(Context.User);
            await Reply(Context.Channel,
                CreateEmbed(Context, $"You have **${userData.Money}**"));
        }
    }
}