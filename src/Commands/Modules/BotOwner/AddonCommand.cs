using System.Linq;
using System.Threading.Tasks;
using Discord;
using Gommon;
using Qmmands;
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
                        .AddField("Description", x.Key.Description).WithDescription(Format.Code(x.Value.Code, "cs")))
                    .ToList();

                if (addonEmbeds.Count is 1) return Ok(addonEmbeds.First());
                return Ok(PaginatedMessage.Builder.New
                    .WithDefaults(Context)
                    .WithPages(addonEmbeds)
                    .WithTitle("All installed addons"));
            }


            return Addon.LoadedAddons.AnyGet(x => x.Key.Name.EqualsIgnoreCase(listOrAddon), 
                out var addon)
                ? Ok(Context.CreateEmbedBuilder().WithTitle($"Addon \"{addon.Key.Name}\"")
                    .AddField("Description", addon.Key.Description)
                    .WithDescription(Format.Code(addon.Value.Code, "cs")))
                : BadRequest(
                    $"The provided addon, \"{listOrAddon}\", was not found. " +
                    $"Try `{Context.GuildData.Configuration.CommandPrefix}addon list` to see every initialized addon.");
        }
    }
}