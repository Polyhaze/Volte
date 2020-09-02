using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gommon;
using JetBrains.Annotations;
using Qmmands;
using Volte.Commands.Checks;
using Volte.Commands.Results;
using Volte.Core.Entities;
using Volte.Core.Helpers;
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode

namespace Volte.Commands.Modules
{
    public sealed class HelpModule : VolteModule
    {
        [Command("Help", "H")]
        [Description("Get help for any of Volte's modules or commands.")]
        [Remarks("help [String]")]
        public Task<ActionResult> HelpAsync([Remainder, OptionalArgument] string moduleOrCommand = null)
        {
            if (moduleOrCommand is null)
            {
                return Ok(new StringBuilder().AppendAllLines(
                    "Hey, I'm Volte! Here's a list of my modules and commands designed to help you out.",
                    $"Use `{Context.GuildData.Configuration.CommandPrefix}help {{moduleName}}` to list all commands in a module, " +
                        $"and `{Context.GuildData.Configuration.CommandPrefix}help {{commandName}}` to show information about a command.")
                    .AppendLine()
                    .AppendLine(
                        $"Available Modules: `{CommandService.GetAllModules().Select(x => x.SanitizeName()).Join("`, `")}`")
                    .AppendLine()
                    .AppendLine(
                        $"Available Commands: `{CommandService.GetAllCommands().Select(x => x.Name).Join("`, `")}`")
                    .ToString());
            }

            var module = GetTargetModule(moduleOrCommand);
            var command = GetTargetCommand(moduleOrCommand);

            var isAdmin = command.IsAdmin() || module.IsAdmin();
            var isMod = command.IsMod() || module.IsMod();
            var isBotOwner = command.IsBotOwner() || module.IsBotOwner();

            var result = "**Permission Level**: ";
            if (isMod)
                result += "Moderator";
            else if (isAdmin)
                result += "Admin";
            else if (isBotOwner)
                result += "Bot Owner";
            else
                result += "Default";

            if (module is null)
            {
                if (command is null)
                {
                    return BadRequest($"{EmojiHelper.X} No matching Module/Command was found.");
                }
                else
                {
                    return Ok(Context.CreateEmbedBuilder(new StringBuilder()
                        .AppendAllLines(
                            $"**Command**: {command.Name}",
                            $"**Module**: {command.Module.SanitizeName()}",
                            result,
                            $"**Description**: {command.Description ?? "No summary provided."}",
                            $"**Valid Command Usages**: {Context.GuildData.Configuration.CommandPrefix}{command.AsPrettyString()}",
                            "[**Valid Arguments**](https://github.com/Ultz/Volte/wiki/Argument-Cheatsheet-V4): " +
                            $"{command.GenerateHelp()}"
                        ).ToString()));
                }
            }
            else
            {
                if (command is null)
                {
                    return None(async () =>
                    {
                        await Context.SendPaginatedMessageAsync(
                            module.Commands.Where(x => !x.Attributes.Any(attr => attr is HiddenAttribute))
                                .Select(x => x.FullAliases[0]).GetPages(15),
                            $"Commands in Module {module.SanitizeName()}");
                    }, false);
                }
                else
                {
                    return BadRequest(new StringBuilder()
                        .AppendAllLines(
                            $"{EmojiHelper.X} Found more than one Module or Command. Results:",
                            $"**{module.SanitizeName()}** Module",
                            $"**{command.Name}** Command"
                        ).ToString());
                }
            }
        }

        [CanBeNull]
        private Module GetTargetModule(string input)
            => CommandService.GetAllModules().FirstOrDefault(x => x.SanitizeName().EqualsIgnoreCase(input));

        [CanBeNull]
        private Command GetTargetCommand(string input)
            => CommandService.GetAllCommands().FirstOrDefault(x => x.FullAliases.ContainsIgnoreCase(input));
    }
}