using System.Net.Http;

namespace Volte.Core.Extensions {
    public static class HttpExtensions {
        public static bool IsImage(this HttpResponseMessage msg) {
            var mime = msg.Content.Headers.ContentType.MediaType;
            return mime.EqualsIgnoreCase("image/png")
                   || mime.EqualsIgnoreCase("image/jpeg")
                   || mime.EqualsIgnoreCase("image/gif");
        }
    }
}