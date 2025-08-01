using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Volte.UI.Avalonia.Controls;

public class CircularImage : CustomImageClipControl
{
    public override Geometry GetClip(Size size) => new EllipseGeometry(new Rect(size));
}