using System.Net.Http;

namespace Volte.Core.Extensions {
    public static class HttpExtensions {
        public static bool IsImage(this HttpResponseMessage msg) {
            var mime = msg.Content.Headers.ContentType.MediaType;
            return mime.Equals("image/png")
                   || mime.Equals("image/jpeg")
                   || mime.Equals("image.gif");
        }
    }
}