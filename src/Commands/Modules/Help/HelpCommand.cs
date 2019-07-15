using System.Linq;
using System.Threading.Tasks;
using Qmmands;
using Volte.Extensions;
using Gommon;

namespace Volte.Commands.Modules.Help
{
    public partial class HelpModule : VolteModule
    {
        [Command("Help", "H")]
        [Description("Shows the commands used for module listing, command listing, and command info.")]
        [Remarks("Usage: |prefix|help")]
        public async Task HelpAsync(string moduleOrCommand = null)
        {
            var data = Db.GetData(Context.Guild);
            if (moduleOrCommand is null)
            {
                var embed = Context.CreateEmbedBuilder()
                    .WithDescription("Hey, I'm Volte! Here's a list of my modules and commands designed to help you out. \n" +
                                     $"Use `{data.Configuration.CommandPrefix}help {{moduleName}}` to list all commands in a module, " +
                                     $"and `{data.Configuration.CommandPrefix}help {{commandName}}` to show information about a command." +
                                     "\n\n" +
                                     $"Available Modules: `{CommandService.GetAllModules().Select(x => x.SanitizeName()).Join("`, `")}`" +
                                     "\n\n" +
                                     $"Available Commands: `{CommandService.GetAllCommands().Select(x => x.Name).Join("`, `")}`");

                await embed.SendToAsync(Context.Channel);
            }
            else
            {

                var module = GetTargetModule(moduleOrCommand);
                var command = GetTargetCommand(moduleOrCommand);

                if (module is null && command is null)
                {
                    await Context.CreateEmbedBuilder().WithDescription($"{EmojiService.X} Specified module/command not found.").SendToAsync(Context.Channel);
                }

                if (module != null && command is null)
                {
                    var commands = $"`{module.Commands.Select(x => x.FullAliases.First()).Join("`, `")}`";
                    await Context.CreateEmbedBuilder().WithDescription(commands).WithTitle($"Commands for {module.SanitizeName()}")
                        .SendToAsync(Context.Channel);
                }

                if (module is null && command != null)
                {
                    await Context.CreateEmbed($"**Command**: {command.Name}\n" +
                                              $"**Module**: {command.Module.SanitizeName()}\n" +
                                              $"**Description**: {command.Description ?? "No summary provided."}\n" +
                                              $"**Usage**: {command.SanitizeRemarks(Context)}")
                        .SendToAsync(Context.Channel);
                }

                if (module != null && command != null)
                {
                    await Context.CreateEmbed($"{EmojiService.X} Found more than one Module or Command. Results:\n" +
                                              $"**{module.SanitizeName()}**\n" +
                                              $"**{command.Name}**")
                        .SendToAsync(Context.Channel);
                }

            }


        }

        private Module GetTargetModule(string input) 
            => CommandService.GetAllModules().FirstOrDefault(x => x.SanitizeName().EqualsIgnoreCase(input));

        private Command GetTargetCommand(string input) 
            => CommandService.GetAllCommands().FirstOrDefault(x => x.Name.EqualsIgnoreCase(input));
    }
}