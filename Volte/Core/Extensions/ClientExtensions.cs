using Discord.WebSocket;

namespace Volte.Core.Extensions
{
    public static class ClientExtensions
    {

        public static string GetInviteUrl(this DiscordSocketClient client)
        {
            return $"https://discordapp.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=8";
        }

    }
}
