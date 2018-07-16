using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using SIVA.Core.Files.Readers;

namespace SIVA.Helpers
{
    internal class DblServerCount
    {
        
        /// <summary>
        ///     Send the bot's user count to DBL. (Discord Bot List)
        /// </summary>
        /// <param name="client"></param>
        /// <returns>Response Content as System.String</returns>
        
        internal static async Task<string> UpdateServerCount(DiscordSocketClient client)
        {
            var webclient = new HttpClient();
            var content = new StringContent($"{{ \"server_count\": {client.Guilds.Count} }}", Encoding.UTF8, "application/json");
            webclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Config.GetDblToken());
            var resp = await webclient.PostAsync($"https://discordbots.org/api/bots/{client.CurrentUser.Id}/stats", content);
            return resp.Content.ReadAsStringAsync().ToString();
        }
    }
}