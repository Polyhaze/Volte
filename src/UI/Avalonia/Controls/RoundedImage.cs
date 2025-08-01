using Avalonia;
using Avalonia.Media;
// ReSharper disable MemberCanBePrivate.Global

namespace Volte.UI.Avalonia.Controls;

public class RoundedImage : CustomImageClipControl
{
    public static readonly AttachedProperty<double> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<RoundedImage, double>(
            "CornerRadius", typeof(RoundedImage), 5);

    public double CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }


    public override Geometry GetClip(Size size) 
        => new RectangleGeometry(new Rect(size), CornerRadius, CornerRadius);
}