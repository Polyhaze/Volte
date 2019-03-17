using System.Linq;
using DSharpPlus.Entities;
using Volte.Data;
using Volte.Discord;
using Volte.Services;

namespace Volte.Extensions
{
    public static class UserExtensions
    {
        private static DatabaseService _db = VolteBot.GetRequiredService<DatabaseService>();

        public static bool IsBotOwner(this DiscordMember user) => Config.Owner.Equals(user.Id);

        public static bool IsGuildOwner(this DiscordMember user)
            => user.Guild.Owner.Id.Equals(user.Id) || IsBotOwner(user);

        public static bool IsModerator(this DiscordMember user)
            => HasRole(user, _db.GetConfig(user.Guild.Id).ModerationOptions.ModRole) ||
               IsAdmin(user) ||
               IsGuildOwner(user);

        public static bool HasRole(this DiscordMember user, ulong roleId) =>
            user.Roles.Select(x => x.Id).Contains(roleId);

        public static bool IsAdmin(this DiscordMember user)
            => HasRole(user, _db.GetConfig(user.Guild).ModerationOptions.AdminRole) ||
               IsGuildOwner(user);

        public static string ToHumanReadable(this DiscordUser user) => user.ToString().Split(' ')[1];
        public static string ToHumanReadable(this DiscordMember user) => user.ToString().Split(' ')[1];
    }
}