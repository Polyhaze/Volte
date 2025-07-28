using Avalonia;
using Avalonia.Media.Imaging;
using FluentAvalonia.UI.Windowing;
using Volte.UI.Avalonia.ViewModels;
using Volte.UI.Helpers;
// ReSharper disable InconsistentNaming

namespace Volte.UI.Avalonia.Views;

public partial class UIShellView : AppWindow
{
    public UIShellView()
    {
        InitializeComponent();
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        using var bitmap = new Bitmap(AvaloniaHelper.OpenResource("icon.ico"));
        VolteLogo.Source = Icon = bitmap.CreateScaledBitmap(new PixelSize(48, 48), BitmapInterpolationMode.None);
        
        DataContext = new UIShellViewModel { View = this };
        
        PageManager.Shared.PropertyChanged += (pm, e) =>
        {
            if (e.PropertyName == nameof(PageManager.Current) && pm is PageManager pageManager)
                Navigation.Content = pageManager.Current?.Content;
        };
    }
}