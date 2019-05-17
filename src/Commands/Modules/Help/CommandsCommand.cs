using System.Linq;
using System.Threading.Tasks;
using Qmmands;
using Volte.Extensions;
using Gommon;

namespace Volte.Commands.Modules.Help
{
    public partial class HelpModule : VolteModule
    {
        [Command("Commands", "Cmds")]
        [Description("Shows commands for a module.")]
        [Remarks("Usage: |prefix|commands {module}")]
        public async Task CommandsAsync(string module)
        {
            var target = CommandService.GetAllModules().FirstOrDefault(m => m.SanitizeName().EqualsIgnoreCase(module));
            if (target is null)
            {
                await Context.CreateEmbed($"{EmojiService.X} Specified module not found.").SendToAsync(Context.Channel);
                return;
            }

            var commands = $"`{target.Commands.Select(x => x.FullAliases.First()).Join("`, `")}`";
            await Context.CreateEmbedBuilder(commands).WithTitle($"Commands for {target.SanitizeName()}")
                .SendToAsync(Context.Channel);
        }
    }
}