using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Utility
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Suggest")]
        [Description("Suggest features for Volte.")]
        [Remarks("Usage: |prefix|suggest")]
        public async Task SuggestAsync()
        {
            await Context.CreateEmbed("You can suggest bot features [here](https://goo.gl/forms/i6pgYTSnDdMMNLZU2).")
                .SendToAsync(Context.Channel);
        }
    }
}