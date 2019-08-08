using System.Threading.Tasks;
using Discord;
 
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        [Command("Suggest")]
        [Description("Suggest features for Volte.")]
        [Remarks("Usage: |prefix|suggest")]
        public Task<ActionResult> SuggestAsync()
        {
            return Ok("You can suggest bot features [here](https://goo.gl/forms/i6pgYTSnDdMMNLZU2).");
        }
    }
}