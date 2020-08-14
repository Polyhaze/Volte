using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qmmands;
using Gommon;
using Volte.Commands.Results;
using Volte.Core.Attributes;
using Volte.Core.Helpers;
using Volte.Interactive;

namespace Volte.Commands.Modules
{
    public sealed class HelpModule : VolteModule
    {
        [Command("Help", "H")]
        [Description("Shows the commands used for module listing, command listing, and command info.")]
        [Remarks("help")]
        public Task<ActionResult> HelpAsync([Remainder] string moduleOrCommand = null)
        {
            if (moduleOrCommand is null)
            {
                return Ok(new StringBuilder()
                    .AppendLine("Hey, I'm Volte! Here's a list of my modules and commands designed to help you out.")
                    .AppendLine(
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

            if (module is null && command is null)
            {
                return BadRequest($"{EmojiHelper.X} No matching Module/Command was found.");
            }

            if (module != null && command is null)
            {
                return Ok(new PaginatedMessageBuilder(Context)
                    .WithTitle($"Commands for {module.SanitizeName()}")
                    .WithContent(result)
                    .WithPages(module.Commands.Select(x => x.FullAliases.First()))
                    .SplitPages(15));
            }

            if (module is null && command != null)
            {
                return Ok(Context.CreateEmbedBuilder().WithDescription(new StringBuilder()
                    .AppendLine($"**Command**: {command.Name}")
                    .AppendLine($"**Module**: {command.Module.SanitizeName()}")
                    .AppendLine(result)
                    .AppendLine($"**Description**: {command.Description ?? "No summary provided."}")
                    .AppendLine($"[**Usage**](https://github.com/Ultz/Volte/wiki/Argument-Cheatsheet): {command.GetUsage(Context)}")
                    .ToString()));
            }

            if (module != null && command != null)
            {
                return BadRequest(new StringBuilder()
                    .AppendLine($"{EmojiHelper.X} Found more than one Module or Command. Results:")
                    .AppendLine($"**{module.SanitizeName()}** Module")
                    .AppendLine($"**{command.Name}** Command")
                    .ToString());
            }

            return None();
        }

        private Module GetTargetModule(string input)
            => CommandService.GetAllModules().FirstOrDefault(x => x.SanitizeName().EqualsIgnoreCase(input));

        private Command GetTargetCommand(string input)
            => CommandService.GetAllCommands().FirstOrDefault(x => x.FullAliases.ContainsIgnoreCase(input));
    }
}