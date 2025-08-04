using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using Gommon;

namespace Volte.UI.Avalonia;

public partial class VolteApp
{
    public static string GetResource(string assetSubdir) => $"avares://Volte.UI/Assets/{assetSubdir}";
    
    public static Uri GetResourceUri(string assetSubdir) => new(GetResource(assetSubdir));
    
    public static Stream OpenResource(Uri asset) => AssetLoader.Open(asset);
    
    public static Stream OpenResource(string assetSubdir) => OpenResource(GetResourceUri(assetSubdir));
    
    public static bool RequestAvaloniaShutdown(int exitCode = 0) 
        => DesktopLifetime?.TryShutdown(exitCode) ?? false;
    
    public static IClassicDesktopStyleApplicationLifetime? DesktopLifetime
        => Current?.ApplicationLifetime?.Cast<IClassicDesktopStyleApplicationLifetime>();

    public static bool TryGetDesktop(out IClassicDesktopStyleApplicationLifetime desktopLifetime)
        => (desktopLifetime = DesktopLifetime!) != null;
}