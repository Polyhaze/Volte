using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Volte.Core.Extensions;
using Volte.Core.Helpers;

namespace Volte.Core.Commands.Modules.Help {
    public partial class HelpModule : VolteModule {
        [Command("Commands"), Alias("Cmds")]
        [Summary("Shows commands for a module.")]
        [Remarks("Usage: |prefix|commands {module}")]
        public async Task Commands(string module) {
            var target = Cs.Modules.FirstOrDefault(m => m.SanitizeName().EqualsIgnoreCase(module));
            if (target is null) {
                await Context.CreateEmbed("Specified module not found.").SendTo(Context.Channel);
                return;
            }
            if ((target.SanitizeName().EqualsIgnoreCase("admin") && !UserUtils.IsAdmin(Context)) || 
                (target.SanitizeName().EqualsIgnoreCase("owner") && !UserUtils.IsBotOwner(Context.User)) || 
                (target.SanitizeName().EqualsIgnoreCase("moderation") && !UserUtils.IsModerator(Context))) {
                await Context.CreateEmbed("You don't have permission to use the module that command is from.")
                    .SendTo(Context.Channel);
                return;
            }

            var commands = target.Commands.Aggregate(string.Empty, (current, command) => current + $"`{command.Name.ToLower()}`, ");
            commands = commands.Remove(commands.LastIndexOf(','));
            await Context.CreateEmbed(commands).ToEmbedBuilder().WithTitle($"Commands for {target.SanitizeName()}")
                .SendTo(Context.Channel);
        }
    }
}