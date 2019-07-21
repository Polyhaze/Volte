using Discord;
using Discord.WebSocket;
using Volte.Data;

namespace Gommon
{
    public static partial class Extensions
    {
        public static string GetInviteUrl(this IDiscordClient client, bool shouldHaveAdmin = true)
        {
            return shouldHaveAdmin
                ? $"https://discordapp.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=8"
                : $"https://discordapp.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=0";
        }

        public static IUser GetOwner(this DiscordShardedClient client) => client.GetUser(Config.Owner);

        public static IGuild GetPrimaryGuild(this DiscordShardedClient client) => client.GetGuild(405806471578648588);
    }
}