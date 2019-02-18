using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Extensions;

namespace Volte.Core.Commands.Modules.Help
{
    public partial class HelpModule : VolteModule
    {
        [Command("Modules", "Mdls")]
        [Description("Lists available modules.")]
        [Remarks("Usage: |prefix|modules")]
        public async Task Modules()
        {
            var modules = CommandService.GetAllModules().Aggregate(string.Empty,
                (current, module) => current + $"`{module.SanitizeName()}`, ");
            await Context.CreateEmbedBuilder(modules.Remove(modules.LastIndexOf(","))).WithTitle("Available Modules")
                .SendTo(Context.Channel);
        }
    }
}