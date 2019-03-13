using Discord;
using Discord.WebSocket;
using Volte.Data;

namespace Volte.Extensions
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

        public static IGuild GetPrimaryGuild(this DiscordSocketClient client)
        {
            return client.GetGuild(405806471578648588);
        }
    }
}