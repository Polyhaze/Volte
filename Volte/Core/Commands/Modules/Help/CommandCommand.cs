using System.Linq;
using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Extensions;
using Volte.Core.Helpers;

namespace Volte.Core.Commands.Modules.Help
{
    public partial class HelpModule : VolteModule
    {
        [Command("Command", "Cmd")]
        [Description("Shows info about a command.")]
        [Remarks("Usage: |prefix|command {cmdName}")]
        public async Task CommandAsync([Remainder] string cmdName)
        {
            var c = CommandService.GetAllCommands().FirstOrDefault(x => x.FullAliases.ContainsIgnoreCase(cmdName));
            if (c is null)
            {
                await Context.CreateEmbed($"{EmojiService.X} Command not found.").SendTo(Context.Channel);
                return;
            }

            if ((c.Module.SanitizeName().EqualsIgnoreCase("admin") && !UserUtils.IsAdmin(Context)) ||
                (c.Module.SanitizeName().EqualsIgnoreCase("owner") && !UserUtils.IsBotOwner(Context.User)) ||
                (c.Module.SanitizeName().EqualsIgnoreCase("moderation") && !UserUtils.IsModerator(Context)))
            {
                await Context.CreateEmbed($"{EmojiService.X} You don't have permission to use the module that command is from.")
                    .SendTo(Context.Channel);
                return;
            }

            await Context.CreateEmbed($"**Command**: {c.Name}\n" +
                                      $"**Module**: {c.Module.SanitizeName()}\n" +
                                      $"**Summary**: {c.Description ?? "No summary provided."}\n" +
                                      $"**Usage**: {c.SanitizeRemarks(Context)}")
                .SendTo(Context.Channel);
        }
    }
}