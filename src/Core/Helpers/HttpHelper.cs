using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Gommon;

namespace Volte.Core.Helpers
{
    public static class HttpHelper
    {
        /// <summary>
        ///     Posts the string <paramref name="content"/> to https://paste.greemdev.net with the <see cref="HttpClient"/> from <paramref name="provider"/>.
        ///     This method will throw if there is no <see cref="HttpClient"/> in the <paramref name="provider"/> given.
        /// </summary>
        /// <param name="content">The content to send.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> containing the <see cref="HttpClient"/>>.</param>
        /// <param name="fileExtension">The file extension {including .} for the resulting URL.</param>
        /// <returns>The URL of the successful paste.</returns>
        /// <exception cref="InvalidOperationException">If <paramref name="provider"/> doesn't have an <see cref="HttpClient"/> in it.</exception>
        public static async Task<string> PostToGreemPasteAsync(string content, IServiceProvider provider, string fileExtension = "")
        {
            var response = await provider.Get<HttpClient>().PostAsync("https://paste.greemdev.net/documents", new StringContent(content, Encoding.UTF8, "text/plain"));
            var jdoc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return $"https://paste.greemdev.net/{jdoc.RootElement.GetProperty("key").GetString()}{fileExtension}";
        }
    }
}