using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Gommon
{
    public partial class Extensions
    {
        
        public static MemoryStream CreateColorImage(this Rgba32 color)
        {
            var @out = new MemoryStream();
            using var image = new Image<Rgba32>(200, 200);
            image.Mutate(a => a.BackgroundColor(color));
            image.SaveAsPng(@out);
            @out.Position = 0;
            return @out;
        }
        
    }
}