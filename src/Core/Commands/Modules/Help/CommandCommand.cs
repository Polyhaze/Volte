using System.Linq;
using System.Threading.Tasks;
using Qmmands;
using Volte.Core.Extensions;
using Volte.Core.Utils;

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
                await Context.CreateEmbed($"{EmojiService.X} Command not found.").SendToAsync(Context.Channel);
                return;
            }

            if (CanShowCommandInfo(c))
            {
                await Context.CreateEmbed($"{EmojiService.X} You don't have permission to use the module that command is from.")
                    .SendToAsync(Context.Channel);
                return;
            }

            await Context.CreateEmbed($"**Command**: {c.Name}\n" +
                                      $"**Module**: {c.Module.SanitizeName()}\n" +
                                      $"**Description**: {c.Description ?? "No summary provided."}\n" +
                                      $"**Usage**: {c.SanitizeRemarks(Context)}")
                .SendToAsync(Context.Channel);
        }

        private bool CanShowCommandInfo(Command c)
        {
            var admin = c.Module.SanitizeName().EqualsIgnoreCase("admin") && !UserUtil.IsAdmin(Context);
            var owner = c.Module.SanitizeName().EqualsIgnoreCase("owner") && !UserUtil.IsBotOwner(Context.User);
            var moderation = c.Module.SanitizeName().EqualsIgnoreCase("moderation") && !UserUtil.IsModerator(Context);
            var serverAdmin = c.Module.SanitizeName().EqualsIgnoreCase("serveradmin") && !UserUtil.IsAdmin(Context);
            return admin || owner || moderation || serverAdmin;
        }
    }
}