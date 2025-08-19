using System.IO;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = Discord.Color;

namespace Gommon;

public partial class Extensions
{
    public static bool LengthEquals(this Stream stream, long exactLength) => stream.Length == exactLength;

    public static MemoryStream CreateColorImage(this Rgba32 color, int width = 125, int height = 125) => new MemoryStream().Apply(ms =>
    {
        using var image = new Image<Rgba32>(width, height);
        image.Mutate(a => a.BackgroundColor(color));
        image.SaveAsPng(ms);
        ms.Position = 0;
    });

    public static Rgba32 ToRgba32(this Color color) => new(color.R, color.G, color.B);
}