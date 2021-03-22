using System;
using System.Collections.Generic;
using System.Linq;
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
            foreach (var cmd in CommandService.GetAllCommands())
            {
                if (!await CommandHelper.CanShowCommandAsync(Context, cmd)) continue;
                var fmt = CommandHelper.FormatCommandShort(cmd);
                if (fmt != null && !commands.Contains(fmt)) commands.Add(fmt);
            }

            try
            {
                if (!commands.IsEmpty())
                    e.AddField("Commands", commands.Join(", "));

            }
            catch (ArgumentException)
            {
                e.WithDescription($"{e.Description}\n\n{commands.Join(", ")}");
            }

            return Ok(e);
        }
    }
}