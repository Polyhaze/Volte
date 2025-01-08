using Avalonia.Controls.Notifications;
using Gommon;
using MenuFactory.Abstractions.Attributes;
using Volte.Interactions;

namespace Volte.UI.Avalonia.Pages;

public class ShellViewMenu
{
    [Menu("Clear Commands", "Dev", Icon = "fa-solid fa-broom")]
    public static async Task ClearCommands()
    {
        var interactionService = VolteBot.Services.Get<VolteInteractionService>();
        if (interactionService is null || VolteBot.Client is null)
        {
            VolteApp.Notify("Not logged in", "State error", NotificationType.Error);
            return;
        }

#if DEBUG
        var removedCommandsText = $"{await interactionService.ClearAllCommandsAsync()} commands";
#else
        var removedCommandsText = $"{await interactionService.ClearAllCommandsAsync()} global commands";
#endif

        VolteApp.Notify(removedCommandsText, "Interaction commands cleared");
    }
}