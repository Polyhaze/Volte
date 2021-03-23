using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qmmands;
using Gommon;
using Volte.Commands;
using Volte.Core.Entities;
using Volte.Core.Helpers;

namespace Volte.Commands.Modules
{
    public sealed class HelpModule : VolteModule
    {
        [Command("Help", "H")]
        [Description("Shows the commands used for module listing, command listing, and command info.")]
        public async Task<ActionResult> HelpAsync([Remainder] string query = null)
        {
            if (query != null)
            {
                var search = CommandService.FindCommands(query).ToList();
                if (search.IsEmpty())
                {
                    return BadRequest($"No command or group found for `{query}`.");
                }

                return Ok(await CommandHelper.CreateCommandEmbedAsync(search.First().Command, Context));
            }

            var e = Context.CreateEmbedBuilder()
                .WithTitle("Commands")
                .WithDescription(
                    $"You can use `{Context.GuildData.Configuration.CommandPrefix}help {{command/group}}` for more details on a command or group.");

            var commands = new List<string>();
            var groupCommands = new List<string>();

            //module with aliases: command group; without: regular module
            foreach (var mdl in CommandService.GetAllModules().Where(x => x.FullAliases.IsEmpty()))
            {
                if (!await CommandHelper.CanShowModuleAsync(Context, mdl)) continue;

                foreach (var cmd in mdl.Commands)
                {
                    var fmt = CommandHelper.FormatCommandShort(cmd);
                    if (fmt != null && !commands.Contains(fmt)) commands.Add(fmt);
                }
            }

            foreach (var mdl in CommandService.GetAllModules().Where(x => !x.FullAliases.IsEmpty()))
            {
                if (!await CommandHelper.CanShowModuleAsync(Context, mdl)) continue;

                var fmt = CommandHelper.FormatModuleShort(mdl);
                if (fmt != null && !groupCommands.Contains(fmt)) groupCommands.Add(fmt);
            }

            try
            {
                if (!commands.IsEmpty())
                    e.AddField("Commands", commands.Join(", "));
            }
            catch (ArgumentException)
            {
                e.WithDescription(new StringBuilder(e.Description).AppendLine().AppendLine()
                    .AppendLine(commands.Join(", "))
                    .ToString());
            }

            try
            {
                if (!groupCommands.IsEmpty())
                    e.AddField("Group Commands", groupCommands.Join(", "));
            }
            catch (ArgumentException)
            {
                e.WithDescription(new StringBuilder(e.Description).AppendLine().AppendLine()
                    .AppendLine(groupCommands.Join(", "))
                    .ToString());
            }

            return Ok(e);
        }
    }
}