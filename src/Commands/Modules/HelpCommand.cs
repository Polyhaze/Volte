using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Gommon;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed class HelpModule : VolteModule
    {
        [Command("Help", "H")]
        [Description("Get help for Volte's many commands.")]
        public async Task<ActionResult> HelpAsync([Remainder, Description("The command or command group to search for.")] string query = null)
        {
            if (query != null)
            {
                var search = CommandService.FindCommands(query);
                if (search.IsEmpty())
                    return BadRequest($"No command or group found for `{query}`.");

                return Ok(await CommandHelper.CreateCommandEmbedAsync(search.First().Command, Context));
            }

            var e = Context.CreateEmbedBuilder()
                .WithTitle("Command Help")
                .WithDescription(
                    $"You can use `{CommandHelper.FormatUsage(Context, CommandService.GetCommand("Help"))}` for more details on a command or group.");

            var cmds = await GetAllRegularCommandsAsync().ToListAsync();
            var groupCmds = await GetAllGroupCommandsAsync().ToListAsync();

            try
            {
                if (!cmds.IsEmpty())
                    e.AddField("Regular Commands", cmds.Join(", "));
            }
            catch (ArgumentException)
            {
                e.WithDescription(new StringBuilder(e.Description)
                    .AppendLine().AppendLine().AppendLine(cmds.Join(", "))
                    .ToString());
            }

            try
            {
                if (!groupCmds.IsEmpty())
                    e.AddField("Group Commands", groupCmds.Join(", "));
            }
            catch (ArgumentException)
            {
                e.WithDescription(new StringBuilder(e.Description)
                    .AppendLine().AppendLine().AppendLine(groupCmds.Join(", "))
                    .ToString());
            }

            return Ok(e);
        }
        
        //module with aliases: command group; without: regular module

        private async IAsyncEnumerable<string> GetAllRegularCommandsAsync()
        {
            foreach (var mdl in CommandService.GetAllModules().Where(x => x.FullAliases.IsEmpty()))
            {
                if (!await CommandHelper.CanShowModuleAsync(Context, mdl)) continue;

                foreach (var cmd in mdl.Commands)
                {
                    var fmt = CommandHelper.FormatCommandShort(cmd);
                    if (fmt != null) yield return fmt;
                }
            }
        }

        private async IAsyncEnumerable<string> GetAllGroupCommandsAsync()
        {
            foreach (var mdl in CommandService.GetAllModules().Where(x => !x.FullAliases.IsEmpty()))
            {
                if (!await CommandHelper.CanShowModuleAsync(Context, mdl)) continue;

                var fmt = CommandHelper.FormatModuleShort(mdl);
                if (fmt != null) yield return fmt;
            }
        }
        
    }
}