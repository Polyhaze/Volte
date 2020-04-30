/*using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core.Attributes;
using Volte.Core.Models;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("DevTest")]
        [Description("Command for developmental testing. This command in its entirety will be commented out for Releases.")]
        [Remarks("devtest")]
        [RequireGuildAdmin]
        public async Task<ActionResult> DevTestAsync()
        {
            var cat = await Context.Guild.CreateCategoryChannelAsync("Test", category =>
            {
                Logger.Debug(LogSource.Module, "CreateCategoryChannelAsync callback has been started.");
                category.Position = 0;
                Logger.Debug(LogSource.Module, "CreateCategoryChannelAsync callback has finished.");
            });

            return None();
        }
    }
}*/
