using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Suggest")]
        [Description("Suggest features for Volte.")]
        [Remarks("suggest")]
        public Task<ActionResult> SuggestAsync() 
            => Ok("You can suggest bot features [here](https://goo.gl/forms/i6pgYTSnDdMMNLZU2).");
    }
}