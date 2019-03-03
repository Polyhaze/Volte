using System;
using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Choose")]
        [Description("Choose an item from a | delimited list.")]
        [Remarks("Usage: |prefix|choose {option1|option2|option3|...}")]
        public async Task ChooseAsync([Remainder] string options)
        {
            var opts = options.Split('|', StringSplitOptions.RemoveEmptyEntries);

            await Context.CreateEmbed($"I choose `{opts[new Random().Next(0, opts.Length)]}`.").SendTo(Context.Channel);
        }
    }
}