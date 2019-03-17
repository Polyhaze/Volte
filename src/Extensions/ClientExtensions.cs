using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace Volte.Extensions
{
    public static class ClientExtensions
    {
        public static string GetInviteUrl(this DiscordClient client, bool shouldHaveAdmin)
        {
            return shouldHaveAdmin is true
                ? $"https://discordapp.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=8"
                : $"https://discordapp.com/oauth2/authorize?client_id={client.CurrentUser.Id}&scope=bot&permissions=0";
        }

        public static Task<DiscordGuild> GetPrimaryGuild(this DiscordClient client)
        {
            return client.GetGuildAsync(405806471578648588);
        }
    }
}