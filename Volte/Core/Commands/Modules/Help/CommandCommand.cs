using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Volte.Core.Extensions;
using Volte.Core.Helpers;

namespace Volte.Core.Commands.Modules.Help {
    public partial class HelpModule : VolteModule {
        [Command("Command"), Alias("Cmd")]
        [Summary("Shows info about a command.")]
        [Remarks("Usage: |prefix|command {cmdName}")]
        public async Task Command([Remainder]string name) {
            var config = Db.GetConfig(Context.Guild);
            var c = Cs.Commands.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name));
            if (c is null) {
                await Context.CreateEmbed("Command not found.").SendTo(Context.Channel);
                return;
            }
            if ((c.Module.SanitizeName().EqualsIgnoreCase("admin") && !UserUtils.IsAdmin(Context)) || 
                (c.Module.SanitizeName().EqualsIgnoreCase("owner") && !UserUtils.IsBotOwner(Context.User)) || 
                (c.Module.SanitizeName().EqualsIgnoreCase("moderation") && !UserUtils.IsModerator(Context))) {
                await Context.CreateEmbed("You don't have permission to use the module that command is from.")
                    .SendTo(Context.Channel);
                return;
            }
            
            var aliases = c.Aliases.Aggregate("(", (current, alias) => current + alias + "|");
            aliases += ")";
            aliases = aliases.Replace("|)", ")");
            

            await Context.CreateEmbed($"**Command**: {c.Name}\n" +
                                      $"**Module**: {c.Module.Name.Replace("Module", string.Empty)}\n" +
                                      $"**Summary**: {c.Summary ?? "No summary provided."}\n" +
                                      "**Usage**: " + (c.Remarks ?? "No usage provided")
                                          .Replace(c.Name.ToLower(), aliases)
                                          .Replace("|prefix|", config.CommandPrefix)
                                          .Replace("Usage: ", string.Empty))
                .SendTo(Context.Channel);

        }
    }
}