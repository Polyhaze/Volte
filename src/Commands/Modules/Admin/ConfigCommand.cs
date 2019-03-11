using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Preconditions;
using Volte.Extensions;

namespace Volte.Commands.Modules.Admin
{
    public partial class AdminModule : VolteModule
    {
        [Command("Config")]
        [Description("Shows a JSON-formatted version of the object for detailed inspection.")]
        [Remarks("Usage: |prefix|config")]
        [RequireGuildAdmin]
        public async Task ConfigAsync()
        {
            await Context.CreateEmbed(DebugService.Execute(Db.GetConfig(Context.Guild).ToString()))
                .SendToAsync(Context.Channel);
        }
    }
}