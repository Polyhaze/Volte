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
                await Context.CreateEmbed($"{EmojiService.X} Specified module not found.").SendToAsync(Context.Channel);
                return;
            }

            if (CanShowModuleInfo(target))
            {
                await Context.CreateEmbed($"{EmojiService.X} You don't have permission to use the module that command is from.")
                    .SendToAsync(Context.Channel);
                return;
            }

            var commands = $"`{string.Join("`, `", target.Commands.Select(x => x.FullAliases.First()))}`";
            await Context.CreateEmbedBuilder(commands).WithTitle($"Commands for {target.SanitizeName()}")
                .SendToAsync(Context.Channel);
        }

        private bool CanShowModuleInfo(Module m)
        {
            var admin = m.SanitizeName().EqualsIgnoreCase("admin") && !UserUtil.IsAdmin(Context);
            var owner = m.SanitizeName().EqualsIgnoreCase("owner") && !UserUtil.IsBotOwner(Context.User);
            var moderation = m.SanitizeName().EqualsIgnoreCase("moderation") && !UserUtil.IsModerator(Context);
            var serverAdmin = m.SanitizeName().EqualsIgnoreCase("serveradmin") && !UserUtil.IsAdmin(Context);
            return admin || owner || moderation || serverAdmin;
        }
    }
}