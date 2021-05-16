using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Gommon;
using Sentry;

namespace Volte.Core.Helpers
{
    public static class HttpHelper
    {
        /// <summary>
        ///     Posts the string <paramref name="content"/> to https://paste.greemdev.net with the <see cref="HttpClient"/> from <paramref name="provider"/>.
        ///     This method will throw if there is no <see cref="HttpClient"/> in the <paramref name="provider"/> given.
        /// </summary>
        /// <param name="content">The content to send.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> containing the <see cref="HttpClient"/>.</param>
        /// <param name="fileExtension">The file extension {including .} for the resulting URL.</param>
        /// <returns>The URL of the successful paste.</returns>
        /// <exception cref="InvalidOperationException">If <paramref name="provider"/> doesn't have an <see cref="HttpClient"/> in it.</exception>
        public static async Task<string> PostToGreemPasteAsync(string content, IServiceProvider provider, string fileExtension = null)
        {
            var response = await provider.Get<HttpClient>().PostAsync("https://paste.greemdev.net/documents", new StringContent(content, Encoding.UTF8, "text/plain"));
            var jdoc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return $"https://paste.greemdev.net/{jdoc.RootElement.GetProperty("key").GetString()}{fileExtension}";
        }
        
        /// <summary>
        ///     Gets a collection of allowed paste sites from https://paste.greemdev.net/volteAllowedPasteSites with the <see cref="HttpClient"/> from <paramref name="provider"/>.
        ///     If any error occurs it is captured via Sentry.
        /// </summary>
        /// <param name="provider">The <see cref="IServiceProvider"/> containing the <see cref="HttpClient"/>.</param>
        /// <returns>An array of strings where each one represents a valid site, or empty if any errors occurred.</returns>
        public static async Task<string[]> GetAllowedPasteSitesAsync(IServiceProvider provider)
        {
            try
            {
                return (await (await provider.Get<HttpClient>().GetAsync("https://paste.greemdev.net/raw/volteAllowedPasteSites")).Content
                    .ReadAsStringAsync()).Split(" ");
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                return Array.Empty<string>();
            }

        }
    }
}