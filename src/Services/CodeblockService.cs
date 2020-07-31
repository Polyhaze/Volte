using System;
using System.Web;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Discord;
using Discord.WebSocket;
using System.Text;

namespace BrackeysBot.Services
{
    public class CodeblockService : BrackeysBotService, IInitializeableService
    {
        [Serializable]
        public struct PasteMystCreateInfo
        {
            [JsonPropertyName("code")]
            public string Code { get; set; }
            [JsonPropertyName("expiresIn")]
            public string ExpiresIn { get; set; }
        }

        [Serializable]
        public struct PasteMystResultInfo
        {
            [JsonPropertyName("id")]
            public string ID { get; set; }
            [JsonPropertyName("createdAt")]
            public long CreatedAt { get; set; }
            [JsonPropertyName("expiresAt")]
            public string ExpiresAt { get; set; }
            [JsonPropertyName("code")]
            public string Code { get; set; }
        }

        private readonly DiscordSocketClient _client;
        private readonly DataService _data;

        private const string API_BASEPOINT = "https://paste.myst.rs/api/";
        private const string PASTEMYST_BASE_URL = "https://paste.myst.rs/";

        private static readonly Regex _codeblockRegex = new Regex(@"^(?:\`){1,3}(\w+?(?:\n))?([^\`]*)\n?(?:\`){1,3}$", RegexOptions.Singleline);

        public CodeblockService(DiscordSocketClient client, DataService data)
        {
            _client = client;
            _data = data;
        }

        public void Initialize()
        {
            _client.MessageReceived += CheckMessage;
        }

        private async Task CheckMessage(SocketMessage sm)
        {
            if (!(sm is SocketUserMessage msg) || msg.Author.IsBot)
                return;

            ulong[] allowedChannels = _data.Configuration.AllowedCodeblockChannelIDs;
            if (allowedChannels != null && allowedChannels.Contains(msg.Channel.Id))
                return;

            if (msg.Content.Length > _data.Configuration.CodeblockThreshold && HasCodeblockFormat(msg.Content))
            {
                string url = await PasteMessage(msg);
                await new EmbedBuilder()
                    .WithAuthor("Pasted!", msg.Author.EnsureAvatarUrl(), url)
                    .WithDescription($"Massive codeblock by {msg.Author.Mention} was pasted!\n[Click here to view it!]({url})")
                    .WithColor(Color.Green)
                    .Build()
                    .SendToChannel(msg.Channel);

                await msg.DeleteAsync();
            }
        }

        public async Task<string> PasteMessage(IMessage msg)
        {
            string code = msg.Content;
            if (HasCodeblockFormat(code))
            {
                code = ExtractCodeblockContent(code);
            }

            PasteMystCreateInfo createInfo = new PasteMystCreateInfo
            {
                Code = HttpUtility.UrlPathEncode(code),
                ExpiresIn = "never"
            };

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(API_BASEPOINT);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Connection.Add(HttpMethod.Post.ToString().ToUpper());

            StringContent content = new StringContent(JsonSerializer.Serialize(createInfo), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync("paste", content);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            PasteMystResultInfo result = JsonSerializer.Deserialize<PasteMystResultInfo>(json);
            Uri pasteUri = new Uri(new Uri(PASTEMYST_BASE_URL), result.ID);

            return pasteUri.ToString();
        }

        private static bool HasCodeblockFormat(string content)
            => _codeblockRegex.IsMatch(content);
        private static string ExtractCodeblockContent(string content)
            => ExtractCodeblockContent(content, out string _);
        private static string ExtractCodeblockContent(string content, out string format)
        {
            Match m = _codeblockRegex.Match(content);
            if (m.Success)
            {
                // If 2 capture is present, the message only contains content, no format
                if (m.Groups.Count == 2)
                {
                    format = string.Empty;
                    return m.Groups[1].Value;
                }
                // If 3 captures are present, the message contains content and a format
                if (m.Groups.Count == 3)
                {
                    format = m.Groups[1].Value;
                    return m.Groups[2].Value;
                }
            }

            format = null;
            return null;
        }
    }
}
