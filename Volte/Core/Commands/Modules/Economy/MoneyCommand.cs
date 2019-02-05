using System.Threading.Tasks;
using Discord.Commands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Economy {
    public partial class EconomyModule : VolteModule {
        [Command("Money"), Alias("$", "Bal")]
        [Summary("Checks how much money you have.")]
        [Remarks("Usage: |prefix|money")]
        public async Task Money() {
            await Context.CreateEmbed($"You have **${Db.GetUser(Context.User).Money}**").SendTo(Context.Channel);
        }
    }
}