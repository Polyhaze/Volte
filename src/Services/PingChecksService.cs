using System.Threading.Tasks;
using Volte.Commands;
using Volte.Discord;
using Volte.Extensions;

namespace Volte.Services
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