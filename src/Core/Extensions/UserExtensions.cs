using System;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core;
using Volte.Services;

namespace Gommon
{
    public static partial class Extensions
    {
        public static bool IsBotOwner(this SocketGuildUser user) => Config.Owner.Equals(user.Id);

        public static bool IsGuildOwner(this SocketGuildUser user)
            => user.Guild.OwnerId.Equals(user.Id) || IsBotOwner(user);

        public static bool IsModerator(this SocketGuildUser user, ServiceProvider provider)
        {
            provider.Get<DatabaseService>(out var db);
            return HasRole(user, db.GetData(user.Guild).Configuration.Moderation.ModRole) ||
                IsAdmin(user, provider) ||
                IsGuildOwner(user);
        }

        public static bool HasRole(this SocketGuildUser user, ulong roleId) => user.Roles.Select(x => x.Id).Contains(roleId);

        public static bool IsAdmin(this SocketGuildUser user, ServiceProvider provider)
        {
            provider.Get<DatabaseService>(out var db);
            return HasRole(user,
                    db.GetData(user.Guild).Configuration.Moderation.AdminRole) ||
                IsGuildOwner(user);
        }
    }
}