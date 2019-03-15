using System.Threading.Tasks;
using Volte.Commands;
using Volte.Extensions;

namespace Volte.Services
{
    [Service("Blacklist", "The main Service for checking messages for blacklisted words/phrases in user's messages.")]
    public sealed class BlacklistService
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