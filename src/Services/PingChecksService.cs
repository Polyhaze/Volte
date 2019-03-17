using System.Threading.Tasks;
using Volte.Commands;
using Volte.Extensions;

namespace Volte.Services
{
    [Service("PingChecks", "The main Service used for checking if any given message contains mass mentions.")]
    public sealed class PingChecksService
    {
        private readonly DatabaseService _db;

        public PingChecksService(DatabaseService databaseService)
        {
            _db = databaseService;
        }

        public async Task CheckMessageAsync(VolteContext ctx)
        {
            var config = _db.GetConfig(ctx.Guild);
            if (config.ModerationOptions.MassPingChecks && !ctx.User.IsAdmin())
            {
                if (ctx.Message.Content.ContainsIgnoreCase("@here") ||
                    ctx.Message.MentionEveryone ||
                    ctx.Message.MentionedUsers.Count > 10)
                {
                    await ctx.Message.DeleteAsync();
                }
            }
        }
    }
}