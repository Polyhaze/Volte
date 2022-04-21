using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Qmmands;
using Gommon;
using Volte.Helpers;
using Volte.Interactive;

namespace Volte.Commands.Modules
{
    public sealed class HelpModule : VolteModule
    {
        [Command("Help", "H")]
        [Description("Get help for Volte's many commands.")]
        public async Task<ActionResult> HelpAsync(
            [Remainder,
             Description(
                 "The command or command group to search for. If you use pages or pager it will list every command in a paginator.")]
            string query = null)
        {
            if (query != null)
            {
                if (query.EqualsAnyIgnoreCase("pages", "pager"))
                    return Ok(await GetPagesAsync().ToListAsync());

                var searchRes = CommandService.GetCommand(query);
                if (searchRes is null)
                    return BadRequest($"No command or group found for {Format.Code(query)}.");

                return Ok(await CommandHelper.CreateCommandEmbedAsync(searchRes, Context));
            }

            var e = Context.CreateEmbedBuilder()
                .WithTitle("Command Help")
                .WithDescription(
                    $"You can use {Format.Code(CommandHelper.FormatUsage(Context, CommandService.GetCommand("Help")))} for more details on a command or group.");

            await GetAllRegularCommandsAsync().ToListAsync().Then(async cmds =>
            {
                await Task.Yield();
                try
                {
                    if (!cmds.IsEmpty()) e.AddField("Regular Commands", cmds.Join(", "));
                }
                catch (ArgumentException)
                {
                    e.AppendDescriptionLine().AppendDescriptionLine(cmds.Join(", "));
                }
            });
            
            await GetAllGroupCommandsAsync().ToListAsync().Then(async groupCmds =>
            {
                await Task.Yield();
                try
                {
                    if (!groupCmds.IsEmpty()) e.AddField("Group Commands", groupCmds.Join(", "));
                }
                catch (ArgumentException)
                {
                    e.AppendDescriptionLine().AppendDescriptionLine(groupCmds.Join(", "));
                }
            });
            
            return Ok(e.WithFooter("Commands missing? Many of my commands have been converted to slash commands. Type / to see them all!"));
        }

        //module without aliases: regular module
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

        // module with aliases: group command module
        private async IAsyncEnumerable<string> GetAllGroupCommandsAsync()
        {
            foreach (var mdl in CommandService.GetAllModules().Where(x => !x.FullAliases.IsEmpty()))
            {
                if (!await CommandHelper.CanShowModuleAsync(Context, mdl)) continue;

                var fmt = CommandHelper.FormatModuleShort(mdl);
                if (fmt != null) yield return fmt;
            }
        }

        private async IAsyncEnumerable<EmbedBuilder> GetPagesAsync() 
        {
            foreach (var cmd in await CommandService.GetAllCommands().WhereAccessibleAsync(Context).ToListAsync())
                yield return await CommandHelper.CreateCommandEmbedAsync(cmd, Context);
        }
    }
}