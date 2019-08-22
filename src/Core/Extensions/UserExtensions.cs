using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Volte.Core;
using Volte.Services;

namespace Gommon
{
    public static partial class Extensions
    {
        public static bool IsBotOwner(this SocketGuildUser user) 
            => Config.Owner == user.Id;

        public static bool IsGuildOwner(this SocketGuildUser user)
            => user.Guild.OwnerId == user.Id || IsBotOwner(user);

        public static bool IsModerator(this SocketGuildUser user, IServiceProvider provider)
        {
            provider.Get<DatabaseService>(out var db);
            return HasRole(user, db.GetData(user.Guild).Configuration.Moderation.ModRole) ||
                IsAdmin(user, provider) ||
                IsGuildOwner(user);
        }

        public static bool HasRole(this SocketGuildUser user, ulong roleId) 
            => user.Roles.Select(x => x.Id).Contains(roleId);

        public static bool IsAdmin(this SocketGuildUser user, IServiceProvider provider)
        {
            provider.Get<DatabaseService>(out var db);
            return HasRole(user,
                    db.GetData(user.Guild).Configuration.Moderation.AdminRole) ||
                IsGuildOwner(user);
        }

        public static async Task<bool> TrySendMessageAsync(this SocketGuildUser user, string text = null, bool isTts = false, Embed embed = null, RequestOptions options = null)
        {
            try
            {
                await user.SendMessageAsync(text, isTts, embed, options);
                return true;
            }
            catch (HttpException e) when (e.HttpCode is HttpStatusCode.Forbidden)
            {
                return false;
            }
        }

        public static async Task<bool> TrySendMessageAsync(this SocketTextChannel channel, string text = null, bool isTts = false, Embed embed = null, RequestOptions options = null)
        {
            try
            {
                await channel.SendMessageAsync(text, isTts, embed, options);
                return true;
            }
            catch (HttpException e) when (e.HttpCode is HttpStatusCode.Forbidden)
            {
                return false;
            }
        }
    }
}