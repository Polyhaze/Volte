using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Commands;
using Volte.Core.Entities;

namespace Volte.Commands.Modules
{
    public partial class BotOwnerModule
    {
        [Command("Addon")]
        [Description("Get an addon or list all addons currently initialized in this instance of Volte.")]
        public Task<ActionResult> AddonAsync([Remainder, Description("An addon's name.")]
            string listOrAddon = "list")
        {
            if (listOrAddon.EqualsIgnoreCase("list"))
            {
                if (Addon.LoadedAddons.IsEmpty())
                    return Ok("You have no addons!\n" +
                              "Addons can be made via making an `addons` directory in my installation folder, " +
                              "and making subfolders with a JSON file with information and a C# file for the addon's logic.");
                return Ok(Context.CreateEmbedBuilder(
                        Addon.LoadedAddons.Select(kvp => $"**{kvp.Key.Name}**: {kvp.Key.Description}").Join("\n")
                    ).WithTitle("All Loaded Addons")
                    .WithFooter(
                        $"To see a specific addon's code, run '{Context.GuildData.Configuration.CommandPrefix}addon {{addonName}}'."));
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