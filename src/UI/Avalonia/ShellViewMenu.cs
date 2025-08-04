using Avalonia.Controls.Notifications;
using Gommon;
using MenuFactory.Abstractions.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Volte.Systems.Interactions;

namespace Volte.UI.Avalonia;

public class ShellViewMenu
{
    /*[Menu("Shutdown", "Tools", Icon = "mdi-power")]
    public static Task ShutdownAsync()
    {
        VolteManager.Stop();
        
        Environment.Exit(0);
        
        return Task.CompletedTask;
    }*/
    
    [Menu("Clear Commands", "Tools", Icon = "fa-solid fa-broom")]
    public static async Task ClearCommands()
    {
        var interactionService = VolteBot.Services.GetService<VolteInteractionService>();
        if (interactionService is null || VolteBot.Client is null)
        {
            VolteApp.Notify("Not logged in", "State error", NotificationType.Error);
            return;
        }

#if DEBUG || !PROD
        var removedCommandsText = $"{await interactionService.ClearAllCommandsAsync()} commands";
#else
        var removedCommandsText = $"{await interactionService.ClearAllCommandsAsync()} global commands";
#endif

        VolteApp.Notify(removedCommandsText, "Interaction commands cleared");
    }
}