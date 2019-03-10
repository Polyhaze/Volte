using Discord;
using Discord.WebSocket;
using Volte.Core.Data;

namespace Volte.Core.Extensions
{
    public static class ClientExtensions
    {
        public static string GetInviteUrl(this IDiscordClient client, bool shouldHaveAdmin)
        {
            return shouldHaveAdmin is true
                ? $"https://discordapp.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=8"
                : $"https://discordapp.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=0";
        }

        public static IUser GetOwner(this DiscordSocketClient client)
        {
            return client.GetUser(Config.Owner);
        }
    }
}