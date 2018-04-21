using SIVA.Core.Bot.Services.Database.DbTypes;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Discord;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SIVA.Core.Bot.Services.Database.Repositories.Impl
{
    public class DiscordUserRepository : Repository<DiscordUser>, IDiscordUserRepository
    {
        public DiscordUserRepository(DbContext context) : base(context)
        {
        }

        public DiscordUser GetOrCreate(ulong userId, uint xp, uint levelNumber, long money)
        {
            _context.Database.ExecuteSqlCommand($@"UPDATE OR IGNORE DiscordUser 
                                                   SET UserId={userId},
                                                       Xp={xp},
                                                       LevelNumber={levelNumber},
                                                       Money={money}
                                                   WHERE UserId={userId};
                                                   INSERT OR IGNORE INTO DiscordUser (UserId, Xp, LevelNumber, Money)
                                                   VALUES ({userId}, {xp}, {levelNumber}, {money});");
            return _set
                .Include(x => x.Id)
                .First(u => u.UserId == userId);
        }

        public DiscordUser GetOrCreate(IUser original)
            => GetOrCreate(original.Id, 0, 0, 0);


        public IEnumerable<DiscordUser> GetRichest(ulong botId, int count, int skip = 0)
        {
            return _set.Where(c => c.Money > 0 && botId != c.UserId)
                .OrderByDescending(c => c.Money)
                .Skip(skip)
                .Take(count)
                .ToList();
        }

        public long GetUserMoney(ulong userId) =>
            _set.FirstOrDefault(x => x.UserId == userId)?.Money ?? 0;


        public void RemoveFromMany(List<long> ids)
        {
            var items = _set.Where(x => ids.Contains((long)x.UserId));
            foreach (var item in items)
            {
                item.Money = 0;
            }
        }
    }
}