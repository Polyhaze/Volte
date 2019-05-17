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
        [Description("Shows commands for a module, or every valid command if `module` is not specified.")]
        [Remarks("Usage: |prefix|commands [module]")]
        public async Task CommandsAsync(string module = null)
        {
            var e = Context.CreateEmbedBuilder();
            if (module is null)
            {
                var mdls = CommandService.GetAllModules();
                foreach (var mdl in mdls)
                {
                    e.AddField(mdl.SanitizeName(), $"`{mdl.Commands.Select(x => x.FullAliases.First()).Join("`, `")}`");
                }

                await e.SendToAsync(Context.Channel);
            }
            else
            {
                var target = CommandService.GetAllModules().FirstOrDefault(m => m.SanitizeName().EqualsIgnoreCase(module));
                if (target is null)
                {
                    await e.WithDescription($"{EmojiService.X} Specified module not found.").SendToAsync(Context.Channel);
                    return;
                }

                var commands = $"`{target.Commands.Select(x => x.FullAliases.First()).Join("`, `")}`";
                await e.WithDescription(commands).WithTitle($"Commands for {target.SanitizeName()}")
                    .SendToAsync(Context.Channel);
            }
        }
    }
}