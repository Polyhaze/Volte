using System.Linq;
using System.Threading.Tasks;
using Gommon;
using Qmmands;
using Volte.Extensions;

namespace Volte.Commands.Modules.Help
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
                await Context.CreateEmbed($"{EmojiService.X} Command not found.").SendToAsync(Context.Channel);
                return;
            }

            await Context.CreateEmbed($"**Command**: {c.Name}\n" +
                                      $"**Module**: {c.Module.SanitizeName()}\n" +
                                      $"**Description**: {c.Description ?? "No summary provided."}\n" +
                                      $"**Usage**: {c.SanitizeRemarks(Context)}")
                .SendToAsync(Context.Channel);
        }
    }
}