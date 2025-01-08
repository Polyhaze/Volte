using Volte.Interactive;

namespace Volte.Commands.Text.Modules;

public partial class BotOwnerModule
{
    [Command("Addons", "Addon")]
    [Description("Get an addon or list all addons currently initialized in this instance of Volte.")]
    public Task<ActionResult> AddonAsync([Remainder, Description("An addon's name.")]
        string listOrAddon = "list")
    {
        if (listOrAddon.EqualsIgnoreCase("list"))
        {
            if (Addon.LoadedAddons.Count == 0)
                return Ok("You have no addons!\n" +
                          $"Addons can be made via making an {Format.Code("addons")} directory in my installation folder, " +
                          $"and {Format.Url("following this", "https://github.com/GreemDev/ExampleVolteAddon")}.");

            var addonEmbeds = Addon.LoadedAddons.Keys.Select(x => Context.CreateEmbedBuilder()
                    .AddField("Name", x.Meta.Name)
                    .AddField("Description", x.Meta.Description).WithDescription(Format.Code(x.Script, "cs")))
                .ToList();

            if (addonEmbeds.Count is 1) return Ok(addonEmbeds.First());
            return Ok(new PaginatedMessage.Builder()
                .WithDefaults(Context)
                .WithPages(addonEmbeds)
                .WithTitle("All installed addons"));
        }


        return Addon.LoadedAddons.Keys.TryGetFirst(x => x.Meta.Name.EqualsIgnoreCase(listOrAddon), out var addon)
            ? Ok(Context.CreateEmbedBuilder().WithTitle($"Addon \"{addon.Meta.Name}\"")
                .AddField("Description", addon.Meta.Description)
                .WithDescription(Format.Code(addon.Script, "cs")))
            : BadRequest(
                $"The provided addon, \"{listOrAddon}\", was not found. " +
                $"Try `{Context.GuildData.Configuration.CommandPrefix}addon list` to see every initialized addon.");
    }
}