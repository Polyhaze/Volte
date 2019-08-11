using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace Gommon
{
    public static partial class Extensions
    {
        private static HttpClient _http = new HttpClient();

        public static async Task<MemoryStream> ToPureColorImageAsync(this Color color)
        {
            var @out = new MemoryStream();
            using var img =
                await _http.GetAsync(
                    "https://raw.githubusercontent.com/abyssal512/Abyss/master/src/Abyss.Core/Assets/transparent_200x200.png");
            await using var imageStream = (await img.Content.ReadAsByteArrayAsync()).ToStream();
            using var image = Image.Load(imageStream);
            image.Mutate(x => x.BackgroundColor(new Rgba32(color.R, color.G, color.B)));
            image.Save(@out, new PngEncoder());
            @out.Position = 0;
            return @out;

        }
    }
}
