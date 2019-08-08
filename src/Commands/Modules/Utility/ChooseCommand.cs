using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Gommon;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Choose")]
        [Description("Choose an item from a list separated by |.")]
        [Remarks("Usage: |prefix|choose {option1|option2|option3|...}")]
        public Task<ActionResult> ChooseAsync([Remainder] string options)
        {
            return Ok($"I choose `{options.Split('|', StringSplitOptions.RemoveEmptyEntries).Random()}`.");
        }
    }
}