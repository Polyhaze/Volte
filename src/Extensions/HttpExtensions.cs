using System.Net.Http;

namespace Volte.Extensions
{
    public static class HttpExtensions
    {
        public static bool IsImage(this HttpResponseMessage msg) =>
            msg.Content.Headers.ContentType.MediaType.StartsWith("image/");
    }
}