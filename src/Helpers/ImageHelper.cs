using System.IO;
using RestSharp;
using RestSharp.Extensions;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace Volte.Helpers
{
    public static class ImageHelper
    {
        private static readonly RestClient Http = new RestClient("https://raw.githubusercontent.com/abyssal512/Abyss/master/src/Abyss.Core/Assets/transparent_200x200.png");
        public static MemoryStream CreateColorImage(Rgba32 color)
        {
            if (!File.Exists("data/transparent.png"))
            {
                Http.DownloadData(new RestRequest(Method.GET)).SaveAs("data/transparent.png");
            }
            var @out = new MemoryStream();
            using var image = Image.Load("data/transparent.png");
            image.Mutate(a => a.BackgroundColor(color));
            image.Save(@out, new PngEncoder());
            @out.Position = 0;
            return @out;
        }
    }
}
