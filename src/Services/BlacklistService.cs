using System.Threading.Tasks;
using Volte.Commands;
using Volte.Extensions;

namespace Volte.Services
{
    public sealed class BlacklistService : IService
    {
        private readonly DatabaseService _db;

        public BlacklistService(DatabaseService databaseService)
        {
            _db = databaseService;
        }

        internal async Task CheckMessageAsync(VolteContext ctx)
        {
            var config = _db.GetConfig(ctx.Guild.Id);

            foreach (var word in config.ModerationOptions.Blacklist)
                if (ctx.Message.Content.ContainsIgnoreCase(word))
                {
                    await ctx.Message.DeleteAsync();
                    return;
                }
        }
    }
}