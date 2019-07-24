using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Suggest")]
        [Description("Suggest features for Volte.")]
        [Remarks("Usage: |prefix|suggest")]
        public Task<VolteCommandResult> SuggestAsync()
        {
            return Ok("You can suggest bot features [here](https://goo.gl/forms/i6pgYTSnDdMMNLZU2).");
        }
    }
}