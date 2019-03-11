using System.Linq;
using System.Threading.Tasks;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Help
{
    public partial class HelpModule : VolteModule
    {
        [Command("Modules", "Mdls")]
        [Description("Lists available modules.")]
        [Remarks("Usage: |prefix|modules")]
        public async Task ModulesAsync()
        {
            var modules = $"`{string.Join("`, `", CommandService.GetAllModules().Select(x => x.SanitizeName()))}`";
            await Context.CreateEmbedBuilder(modules.Remove(modules.LastIndexOf(","))).WithTitle("Available Modules")
                .SendToAsync(Context.Channel);
        }
    }
}