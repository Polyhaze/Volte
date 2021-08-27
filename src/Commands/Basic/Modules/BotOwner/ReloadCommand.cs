using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Gommon;
using Qmmands;
using Volte.Core;
using Volte.Core.Helpers;
using Volte.Services;

namespace Volte.Commands.Modules
{
    public sealed partial class BotOwnerModule
    {
        [Command("Reload", "Rl")]
        [Description(
            "Repopulates AllowedPasteSites and reloads the bot's configuration file if you've changed it. Note: if the file's content is invalid JSON things might start acting up.")]
        public async Task<ActionResult> ReloadAsync()
        {
            AdminUtilityModule.AllowedPasteSites = await HttpHelper.GetAllowedPasteSitesAsync(Context.Services);
            return Config.Reload()
                ? Ok("Config and AllowedPasteSites reloaded!")
                : BadRequest("Something bad happened. Check console for more detailed information.");
        }

        [Command("UpdateCommands", "Uc")]
        [Description("Registers all of the known Slash Commands to Discord.")]
        public async Task<ActionResult> UpdateCommandsAsync()
        {
            try
            {
                var commands = await SlashCommands.UpdateCommandsAsync();
                return Ok(Context.CreateEmbedBuilder(commands.Select(cmd => cmd.Name).ToReadableString())
                        .WithTitle("Commands update successful.").Apply(eb =>
                        {
                            // ReSharper disable once ConditionIsAlwaysTrueOrFalse IsExpressionAlwaysTrue
                            // it's not; SlashCommandService has 2 possible UpdateCommandsAsync, one dev, one prod
                            eb.WithFooter(
                                commands is IReadOnlyCollection<RestGuildCommand>
                                    ? "NOTE: DEBUG: Global commands wiped, all commands made guild commands for the Volte guild."
                                    : "NOTE: Commands may take up to an hour to propagate.");
                        }));
            }
            catch (Exception e)
            {
                return BadRequest(Context.CreateEmbedBuilder().WithTitle("Command update failed.")
                    .WithDescription(e.InnerException?.Message ?? e.Message));
            }
        }
    }
}