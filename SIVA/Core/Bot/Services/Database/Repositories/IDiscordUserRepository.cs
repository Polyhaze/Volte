using Discord;
using SIVA.Core.Bot.Services.Database.DbTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIVA.Core.Bot.Services.Database.Repositories
{
    public interface IDiscordUserRepository : IRepository<DiscordUser>
    {
        DiscordUser GetOrCreate(IUser original);
        long GetUserMoney(ulong id);
        void RemoveFromMany(List<long> ids);
        IEnumerable<DiscordUser> GetRichest(ulong botId, int count, int skip);
    }
}