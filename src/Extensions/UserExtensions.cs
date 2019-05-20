using System.Linq;
using Discord;
using Volte.Data;
using Volte.Core;
using Volte.Services;

namespace Volte.Extensions
{
    public static class UserExtensions
    {
        private static DatabaseService _db = VolteBot.GetRequiredService<DatabaseService>();

        public static bool IsBotOwner(this IGuildUser user) => Config.Owner.Equals(user.Id);

        public static bool IsGuildOwner(this IGuildUser user)
            => user.Guild.OwnerId.Equals(user.Id) || IsBotOwner(user);

        public static bool IsModerator(this IGuildUser user)
            => HasRole(user, _db.GetData(user.Guild.Id).ModerationOptions.ModRole) ||
               IsAdmin(user) ||
               IsGuildOwner(user);

        public static bool HasRole(this IGuildUser user, ulong roleId) => user.RoleIds.Contains(roleId);

        public static bool IsAdmin(this IGuildUser user)
            => HasRole(user, _db.GetData(user.Guild).ModerationOptions.AdminRole) ||
               IsGuildOwner(user);
    }
}