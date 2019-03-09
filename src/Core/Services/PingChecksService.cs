using System.Threading.Tasks;
using Volte.Core.Commands;
using Volte.Core.Discord;
using Volte.Core.Extensions;

namespace Volte.Core.Services
{
    public sealed class PingChecksService : IService
    {
        public async Task CheckMessageAsync(VolteContext ctx)
        {
            var config = VolteBot.GetRequiredService<DatabaseService>().GetConfig(ctx.Guild);
            if (config.ModerationOptions.MassPingChecks && !ctx.User.IsAdmin())
            {
                var content = ctx.Message.Content;
                if (content.ContainsIgnoreCase("@everyone") ||
                    content.ContainsIgnoreCase("@here") ||
                    ctx.Message.MentionedUserIds.Count > 10)
                {
                    await ctx.Message.DeleteAsync();
                }
            }
            
        }
    }
}