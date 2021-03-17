using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Commands.Results;
using Volte.Core.Entities;

namespace Volte.Commands.Modules
{
    public partial class BotOwnerModule
    {
        [Command("Addon")]
        [Description("Get an addon or list all addons currently initialized in this instance of Volte.")]
        [Remarks("addon {list/AddonName}")]
        [RequireBotOwner]
        public Task<ActionResult> AddonAsync([Remainder] string listOrAddon)
        {
            if (listOrAddon.EqualsIgnoreCase("list"))
            {
                if (Addon.LoadedAddons.IsEmpty()) return Ok("You have no addons!");
                return Ok(Context.CreateEmbedBuilder(
                    Addon.LoadedAddons.Select(kvp => $"**{kvp.Key.Name}**: {kvp.Key.Description}").Join("\n")
                ).WithTitle("All LoadedAddons"));
            }

            if (Addon.LoadedAddons.Any(x => x.Key.Name.EqualsIgnoreCase(listOrAddon)))
            {
                var (meta, code) = Addon.LoadedAddons.FirstOrDefault(x => x.Key.Name.EqualsIgnoreCase(listOrAddon));
                return Ok(Context.CreateEmbedBuilder().WithTitle($"Addon \"{meta.Name}\"")
                    .AddField("Description", meta.Description).WithDescription(Format.Code(code, "cs")));
            }

            return BadRequest(
                $"The provided addon, \"{listOrAddon}\", was not found. Try `{Context.GuildData.Configuration.CommandPrefix}addon list` to see every initialized addon.");
        }
    }
}