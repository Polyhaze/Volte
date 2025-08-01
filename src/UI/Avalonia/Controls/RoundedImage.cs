using Avalonia;
using Avalonia.Media;
// ReSharper disable MemberCanBePrivate.Global

namespace Volte.UI.Avalonia.Controls;

public class RoundedImage : CustomImageClipControl
{
    public static readonly AttachedProperty<double> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<RoundedImage, double>(
            "CornerRadius", typeof(RoundedImage), 5);
    
    public static void SetCornerRadius(AvaloniaObject element, double parameter) => element.SetValue(CornerRadiusProperty, parameter);

    public static double GetCornerRadius(AvaloniaObject element) => element.GetValue(CornerRadiusProperty);


    public override Geometry GetClip(Size size) 
        => new RectangleGeometry(new Rect(size), GetCornerRadius(this), GetCornerRadius(this));
}