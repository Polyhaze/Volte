using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Volte.Data;
using Volte.Extensions;
using Color = System.Drawing.Color;

namespace Volte.Services
{
    [Service("ImageWelcome", "The main Service used for getting images from the welcome image API to be used for welcoming when the WelcomeApiKey config entry is set.")]
    public sealed class ImageWelcomeService
    {
        private readonly DatabaseService _db;
        private readonly string _url;
        private static readonly HttpClient HttpClient = new HttpClient();

        public ImageWelcomeService(DatabaseService databaseService)
        {
            _db = databaseService;
            _url = "https://ourmainfra.me/api/v2/welcomer/" +
                   "?avatar={avatarUrl}" +
                   "&user_name={username}%23{discrim}" +
                   "&guild_name={serverName}" +
                   "&member_count={memberCount}" +
                   "&color={hex}" +
                   "&type={type}" +
                   "&Authorization={auth}";
        }

        private string FormatUrl(DiscordMember user)
        {
            var config = _db.GetConfig(user.Guild);
            var c = Color.FromArgb(config.WelcomeOptions.WelcomeColorR, config.WelcomeOptions.WelcomeColorG,
                config.WelcomeOptions.WelcomeColorB);
            var color = string.Concat(c.R.ToString("X2"), c.G.ToString("X2"), c.G.ToString("X2"));
            return _url.Replace("{avatarUrl}", user.AvatarUrl)
                .Replace("{username}%23{discrim}", $"{user.Username}%23{user.Discriminator}")
                .Replace("{serverName}", user.Guild.Name)
                .Replace("{memberCount}", $"{user.Guild.MemberCount}")
                .Replace("{hex}", color)
                .Replace("{type}", "1")
                .Replace("{auth}", Config.WelcomeApiKey);
        }

        internal async Task JoinAsync(DiscordMember user)
        {
            var img = (await
                (await HttpClient.GetAsync(FormatUrl(user), HttpCompletionOption.ResponseHeadersRead)
                ).Content.ReadAsByteArrayAsync()).ToStream();
            var c = user.Guild.Channels.FirstOrDefault(x =>
                x.Id == _db.GetConfig(user.Guild).WelcomeOptions.WelcomeChannel);
            if (c is null) return;
            await c.SendFileAsync($"welcome-{user.Id}.png", img);
        }
    }
}