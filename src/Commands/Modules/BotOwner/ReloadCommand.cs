using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Helpers;

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
                var globalCommands = await Interactions.CommandUpdater.OverwriteGlobalCommandsAsync();

                var newGuildCommands = await Interactions.CommandUpdater.ForceOverwriteAllGuildCommandsAsync();
                
                return Ok(Context.CreateEmbedBuilder().WithTitle("Commands update successful.")
                    .AddField("Total global commands", globalCommands.Count)
                    .AddField("Total guild commands", newGuildCommands.Sum(x => x.Count)));
            }
            catch (Exception e)
            {
                return BadRequest(Context.CreateEmbedBuilder().WithTitle("Command update failed.")
                    .WithDescription(e.GetInnermostException().Message));
            }
        }
    }
}