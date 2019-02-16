using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Volte.Core.Extensions;
using Volte.Core.Helpers;

namespace Volte.Core.Commands.Modules.Help
{
    public partial class HelpModule : VolteModule
    {
        [Command("Commands", "Cmds")]
        [Description("Shows commands for a module.")]
        [Remarks("Usage: |prefix|commands {module}")]
        public async Task Commands(string module)
        {
            var target = CommandService.GetAllModules().FirstOrDefault(m => m.SanitizeName().EqualsIgnoreCase(module));
            if (target is null)
            {
                await Context.CreateEmbed("Specified module not found.").SendTo(Context.Channel);
                return;
            }

            if ((target.SanitizeName().EqualsIgnoreCase("admin") && !UserUtils.IsAdmin(Context)) ||
                (target.SanitizeName().EqualsIgnoreCase("owner") && !UserUtils.IsBotOwner(Context.User)) ||
                (target.SanitizeName().EqualsIgnoreCase("moderation") && !UserUtils.IsModerator(Context)))
            {
                await Context.CreateEmbed("You don't have permission to use the module that command is from.")
                    .SendTo(Context.Channel);
                return;
            }

            var commands = $"`{string.Join("`, `", target.Commands)}`";
            await Context.CreateEmbed(commands).ToEmbedBuilder().WithTitle($"Commands for {target.SanitizeName()}")
                .SendTo(Context.Channel);
        }
    }
}