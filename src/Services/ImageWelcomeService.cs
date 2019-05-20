using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Volte.Data;
using Volte.Data.Models.EventArgs;
using Gommon;
using Color = System.Drawing.Color;

namespace Volte.Services
{
    [Service("ImageWelcome",
        "The main Service used for getting images from the welcome image API to be used for welcoming when the WelcomeApiKey config entry is set.")]
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

        private async Task<string> FormatUrl(IGuildUser user)
        {
            var data = _db.GetData(user.Guild);
            var c = Color.FromArgb(config.WelcomeOptions.WelcomeColorR, config.WelcomeOptions.WelcomeColorG,
                config.WelcomeOptions.WelcomeColorB);
            var color = string.Concat(c.R.ToString("X2"), c.G.ToString("X2"), c.G.ToString("X2"));
            return _url.Replace("{avatarUrl}", user.GetAvatarUrl())
                .Replace("{username}%23{discrim}", $"{user.Username}%23{user.Discriminator}")
                .Replace("{serverName}", user.Guild.Name)
                .Replace("{memberCount}", $"{(await user.Guild.GetUsersAsync()).Count}")
                .Replace("{hex}", color)
                .Replace("{type}", "1")
                .Replace("{auth}", Config.WelcomeApiKey);
        }

        internal async Task JoinAsync(UserJoinedEventArgs args)
        {
            var img = (await
                (await HttpClient.GetAsync(await FormatUrl(args.User), HttpCompletionOption.ResponseHeadersRead)
                ).Content.ReadAsByteArrayAsync()).ToStream();
            var c = await args.Guild.GetTextChannelAsync(args.Config.WelcomeOptions.WelcomeChannel);
            await c.SendFileAsync(img, $"welcome-{args.User.Id}.png", string.Empty);
        }
    }
}