using System;
using System.Threading.Tasks;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Choose")]
        [Description("Choose an item from a list separated by |.")]
        [Remarks("Usage: |prefix|choose {option1|option2|option3|...}")]
        public async Task ChooseAsync([Remainder] string options)
        {
            var opts = options.Split('|', StringSplitOptions.RemoveEmptyEntries);

            await Context.CreateEmbed($"I choose `{opts.Random()}`.")
                .SendToAsync(Context.Channel);
        }
    }
}