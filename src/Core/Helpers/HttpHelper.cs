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
        ///     POSTs the string <paramref name="content"/> to https://paste.greemdev.net with the <see cref="HttpClient"/> from <paramref name="provider"/>.
        ///     This method will throw if there is no <see cref="HttpClient"/> in the <paramref name="provider"/> given.
        /// </summary>
        /// <param name="content">The content to send.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> containing the <see cref="HttpClient"/>.</param>
        /// <param name="fileExtension">The file extension {WITHOUT PRECEDING PERIOD} for the resulting URL.</param>
        /// <returns>The URL of the successful paste.</returns>
        /// <exception cref="InvalidOperationException">If <paramref name="provider"/> doesn't have an <see cref="HttpClient"/> in it.</exception>
        public static async Task<string> PostToGreemPasteAsync(string content, IServiceProvider provider, string fileExtension = null)
        {
            const string url = "https://paste.greemdev.net/documents";
            try
            {
                var jdoc = JsonDocument.Parse(await (await PostStringAsync(provider, url, content)).Content.ReadAsStringAsync());
                return $"https://paste.greemdev.net/{jdoc.RootElement.GetProperty("key").GetString()}{(fileExtension is null ? "" : $".{fileExtension}")}}}";
            }
            catch (Exception e)
            {
                e.Data["url"] = url;
                e.Data["fileExtension"] = fileExtension; // Useful in case the exception was thrown on the return line
                SentrySdk.CaptureException(e);
                return string.Empty;
            }

        }

        /// <summary>
        ///     POSTs the specified string <paramref name="content"/> to <paramref name="url"/> with the <see cref="HttpClient"/> from <paramref name="provider"/>.
        /// </summary>
        /// <param name="provider">The <see cref="IServiceProvider"/> containing the <see cref="HttpClient"/>.</param>
        /// <param name="url">The URL to POST to.</param>
        /// <param name="content">The content to POST.</param>
        /// <returns>The resulting <see cref="HttpResponseMessage"/>.</returns>
        public static Task<HttpResponseMessage> PostStringAsync(IServiceProvider provider, string url, string content) 
            => provider.Get<HttpClient>().PostAsync(url, new StringContent(content, Encoding.UTF8, "text/plain"));
        
        
        /// <summary>
        ///     Gets a collection of allowed paste sites from https://paste.greemdev.net/volteAllowedPasteSites with the <see cref="HttpClient"/> from <paramref name="provider"/>.
        ///     If any error occurs it is captured via Sentry, and the method returns an empty array.
        /// </summary>
        /// <param name="provider">The <see cref="IServiceProvider"/> containing the <see cref="HttpClient"/>.</param>
        /// <returns>An array of strings where each one represents a valid site, or empty if any errors occurred.</returns>
        public static async Task<string[]> GetAllowedPasteSitesAsync(IServiceProvider provider)
        {
            try
            {
                const string url = "https://paste.greemdev.net/raw/volteAllowedPasteSites";
                return (await (await provider.Get<HttpClient>().GetAsync(url)).Content
                    .ReadAsStringAsync()).Split(" ");
            }
            catch (Exception e)
            {
                e.Data["url"] = url;
                SentrySdk.CaptureException(e);
                return Array.Empty<string>();
            }

        }
    }
}
