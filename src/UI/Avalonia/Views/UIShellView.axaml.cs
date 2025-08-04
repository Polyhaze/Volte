using Avalonia;
using Avalonia.Media.Imaging;
using FluentAvalonia.UI.Windowing;
using Volte.UI.Avalonia.ViewModels;

// ReSharper disable InconsistentNaming

namespace Volte.UI.Avalonia.Views;

public partial class UIShellView : AppWindow
{
    public UIShellView()
    {
        InitializeComponent();
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        using var bitmap = new Bitmap(VolteApp.OpenResource("icon.ico"));
        VolteLogo.Source = Icon = bitmap.CreateScaledBitmap(new PixelSize(48, 48), BitmapInterpolationMode.None);
        
        DataContext = new UIShellViewModel();
    }
}