namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class BotOwnerModule
{
    [Command("Shutdown")]
    [Description("Forces the bot to shutdown.")]
    public Task<ActionResult> ShutdownAsync()
        => Ok($"Goodbye! {Emojis.Wave}", 
            _ => Cts.CancelAsync());
}