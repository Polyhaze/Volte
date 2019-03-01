using System.Linq;
using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Extensions;
using Volte.Core.Utils;

namespace Volte.Core.Commands.Modules.Help
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
                await Context.CreateEmbed($"{EmojiService.X} Specified module not found.").SendTo(Context.Channel);
                return;
            }

            if ((target.SanitizeName().EqualsIgnoreCase("admin") && !UserUtil.IsAdmin(Context)) ||
                (target.SanitizeName().EqualsIgnoreCase("owner") && !UserUtil.IsBotOwner(Context.User)) ||
                (target.SanitizeName().EqualsIgnoreCase("moderation") && !UserUtil.IsModerator(Context)))
            {
                await Context.CreateEmbed($"{EmojiService.X} You don't have permission to use the module that command is from.")
                    .SendTo(Context.Channel);
                return;
            }

            var commands = $"`{string.Join("`, `", target.Commands)}`";
            await Context.CreateEmbedBuilder(commands).WithTitle($"Commands for {target.SanitizeName()}")
                .SendTo(Context.Channel);
        }
    }
}