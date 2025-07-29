namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class UtilityModule
{
    [Command("Choose")]
    [Description("Randomly choose an item from a list separated by |.")]
    public Task<ActionResult> ChooseAsync(
        [Remainder, Description("The options you want to choose from; separated by `|`.")]
        string options)
        => Ok($"I choose {Format.Code(options.Split('|', StringSplitOptions.RemoveEmptyEntries).GetRandomElement())}.");
}