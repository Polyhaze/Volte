using Volte.UI.Avalonia;

namespace Volte.UI.Helpers;

// ReSharper disable once InconsistentNaming
public static class OS
{
    public static async Task CopyToClipboardAsync(string content)
    {
        if (VolteApp.DesktopLifetime?.MainWindow?.Clipboard is { } clipboard)
            await clipboard.SetTextAsync(content);
    }
    
}