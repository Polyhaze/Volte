using System.Linq;
using System.Threading.Tasks;
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
            var admin = c.Module.SanitizeName().EqualsIgnoreCase("admin") && !Context.User.IsAdmin();
            var serverAdmin = c.Module.SanitizeName().EqualsIgnoreCase("serveradmin") && !Context.User.IsAdmin();
            var owner = c.Module.SanitizeName().EqualsIgnoreCase("owner") && !Context.User.IsBotOwner();
            var moderation = c.Module.SanitizeName().EqualsIgnoreCase("moderation") && !Context.User.IsModerator();
            return admin || serverAdmin || owner || moderation;
        }
    }
}