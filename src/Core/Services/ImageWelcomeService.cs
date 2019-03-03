using System.Net.Http;
using System.Threading.Tasks;
using Discord.WebSocket;
using Volte.Core.Discord;
using System.Drawing;
using Volte.Core.Data;
using Volte.Core.Extensions;

namespace Volte.Core.Services
{
    public sealed class ImageWelcomeService : IService
    {
        private readonly DatabaseService _db = VolteBot.GetRequiredService<DatabaseService>();
        private readonly string _url = "https://ourmainfra.me/api/v2/welcomer/" +
                                       "?avatar={avatarUrl}" +
                                       "&user_name={username}%23{discrim}" +
                                       "&guild_name={serverName}" +
                                       "&member_count={memberCount}" +
                                       "&color={hex}" +
                                       "&type={type}" +
                                       "&Authorization={auth}";
        private static readonly HttpClient HttpClient = new HttpClient();

        private string FormatUrl(SocketGuildUser user)
        {
            var config = _db.GetConfig(user.Guild);
            var c = Color.FromArgb(config.WelcomeOptions.WelcomeColorR, config.WelcomeOptions.WelcomeColorG, config.WelcomeOptions.WelcomeColorB);
            var color = string.Concat(c.R.ToString("X2"), c.G.ToString("X2"), c.G.ToString("X2"));
            return _url.Replace("{avatarUrl}", user.GetAvatarUrl())
                .Replace("{username}%23{discrim}", $"{user.Username}%23{user.Discriminator}")
                .Replace("{serverName}", user.Guild.Name)
                .Replace("{memberCount}", $"{user.Guild.Users.Count}")
                .Replace("{hex}", color)
                .Replace("{type}", "1")
                .Replace("{auth}", Config.WelcomeApiKey);

        }

        internal async Task JoinAsync(SocketGuildUser user)
        {
            var img = (await 
                (await HttpClient.GetAsync(FormatUrl(user), HttpCompletionOption.ResponseHeadersRead)
                ).Content.ReadAsByteArrayAsync()).ToStream();
            var channelId = _db.GetConfig(user.Guild).WelcomeOptions.WelcomeChannel;
            var c = user.Guild.GetTextChannel(channelId);
            await c.SendFileAsync(img, $"welcome-{user.Id}.png", string.Empty);
        }
    }
}