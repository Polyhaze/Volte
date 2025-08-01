using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Volte.UI.Avalonia.Controls;

public abstract class CustomImageClipControl : Image
{
    public abstract Geometry GetClip(Size resultSize);
    
    protected override Size MeasureOverride(Size availableSize)
    {
        IImage? source = Source;
        Size result = new();

        if (source != null)
        {
            result = Stretch.CalculateSize(availableSize, source.Size, StretchDirection);
        }

        Clip = GetClip(result);
        return result;
    }
}