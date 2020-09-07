using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Volte.Services
{
    public class HttpService : VolteService
    {
        private readonly HttpClient _http;
        public HttpService(HttpClient http)
        {
            _http = http;
        }
        
        /// <summary>
        ///     Posts the specified text to paste.greemdev.net.
        ///     This will only fail if paste.greemdev.net is offline.
        /// </summary>
        /// <param name="content">The content to post</param>
        /// <returns>The resulting paste.greemdev.net URL.</returns>
        public async Task<string> PostToGreemPasteAsync(string content)
        {
            var response = await _http.PostAsync("https://paste.greemdev.net/documents", new StringContent(content, Encoding.UTF8, "text/plain"));
            var jdoc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return $"https://paste.greemdev.net/{jdoc.RootElement.GetProperty("key").GetString()}.cs";
        }
    }
}