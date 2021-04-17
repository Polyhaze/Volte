using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
using Volte.Commands;
using Volte.Core.Entities;
using Volte.Interactive;

namespace Volte.Commands.Modules
{
    public partial class BotOwnerModule
    {
        [Command("Addons", "Addon")]
        [Description("Get an addon or list all addons currently initialized in this instance of Volte.")]
        public Task<ActionResult> AddonAsync([Remainder, Description("An addon's name.")]
            string listOrAddon = "list")
        {
            if (listOrAddon.EqualsIgnoreCase("list"))
            {
                if (Addon.LoadedAddons.IsEmpty())
                    return Ok("You have no addons!\n" +
                              $"Addons can be made via making an {Format.Code("addons")} directory in my installation folder, " +
                              $"and {Format.Url("following this", "https://github.com/GreemDev/ExampleVolteAddon")}.");

                var addonEmbeds = Addon.LoadedAddons.Select(x => Context.CreateEmbedBuilder()
                        .AddField("Name", x.Key.Name)
                        .AddField("Description", x.Key.Description).WithDescription(Format.Code(x.Value, "cs")))
                    .ToList();

                if (addonEmbeds.Count is 1) return Ok(addonEmbeds.First());
                return Ok(PaginatedMessage.Builder.New
                    .WithDefaults(Context)
                    .WithPages(addonEmbeds)
                    .WithTitle("All installed addons"));
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