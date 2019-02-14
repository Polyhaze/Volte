using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Economy
{
    public partial class EconomyModule : VolteModule
    {
        [Command("Money", "$", "Bal")]
        [Description("Checks how much money you have.")]
        [Remarks("Usage: |prefix|money")]
        public async Task Money()
        {
            await Context.CreateEmbed($"You have **${Db.GetUser(Context.User).Money}**").SendTo(Context.Channel);
        }
    }
}