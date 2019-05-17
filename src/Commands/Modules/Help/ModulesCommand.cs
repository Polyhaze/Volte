using System.Linq;
using System.Threading.Tasks;
using Qmmands;
using Volte.Extensions;
using Gommon;

namespace Volte.Commands.Modules.Help
{
    public partial class HelpModule : VolteModule
    {
        [Command("Modules", "Mdls")]
        [Description("Lists available modules.")]
        [Remarks("Usage: |prefix|modules")]
        public async Task ModulesAsync()
        {
            var modules = $"`{CommandService.GetAllModules().Select(x => x.SanitizeName()).Join("`, `")}`";
            await Context.CreateEmbedBuilder(modules).WithTitle("Available Modules")
                .SendToAsync(Context.Channel);
        }
    }
}