using System;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Data.Models.Results;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Choose")]
        [Description("Choose an item from a list separated by |.")]
        [Remarks("Usage: |prefix|choose {option1|option2|option3|...}")]
        public Task<VolteCommandResult> ChooseAsync([Remainder] string options)
        {
            var opts = options.Split('|', StringSplitOptions.RemoveEmptyEntries);

            return Ok($"I choose `{opts.Random()}`.");
        }
    }
}