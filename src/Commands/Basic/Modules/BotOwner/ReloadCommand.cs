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
                var restCommands = await SlashCommands.UpdateCommandsAsync();
                var (slashCommands, userCommands, messageCommands) = (
                    restCommands.Where(x => x.Type is ApplicationCommandType.Slash).ToArray(),
                    restCommands.Where(x => x.Type is ApplicationCommandType.User).ToArray(),
                    restCommands.Where(x => x.Type is ApplicationCommandType.Message).ToArray()
                );
                return Ok(Context.CreateEmbedBuilder().WithTitle("Commands update successful.")
                        .Apply(eb =>
                        {
                            if (!slashCommands.IsEmpty())
                                eb.AddField("Slash Commands", slashCommands.Select(x => x.Name).ToReadableString(), true);
                            if (!userCommands.IsEmpty())
                                eb.AddField("User Commands", userCommands.Select(x => x.Name).ToReadableString(), true);
                            if (!messageCommands.IsEmpty())
                                eb.AddField("Message Commands", messageCommands.Select(x => x.Name).ToReadableString(), true);

                            if (eb.Fields.IsEmpty())
                            {
                                eb.WithDescription("There were no commands updated.");
                                return;
                            }
                            
                            // ReSharper disable once ConditionIsAlwaysTrueOrFalse IsExpressionAlwaysTrue
                            // it's not; SlashCommandService has 2 possible UpdateCommandsAsync, one dev, one prod
                            eb.WithFooter(
#if DEBUG
                                    "NOTE: DEBUG: Global commands wiped, all commands made guild commands for the Volte guild."
#else
                                    "NOTE: Commands may take up to an hour to propagate."
#endif
                            );
                        }));
            }
            catch (Exception e)
            {
                return BadRequest(Context.CreateEmbedBuilder().WithTitle("Command update failed.")
                    .WithDescription(e.GetInnermostException().Message));
            }
        }
    }
}