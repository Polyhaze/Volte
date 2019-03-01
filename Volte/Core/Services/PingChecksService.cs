using System.Threading.Tasks;
using Volte.Core.Commands;
using Volte.Core.Discord;
using Volte.Core.Extensions;
using Volte.Core.Utils;

namespace Volte.Core.Services
{
    public class PingChecksService : IService
    {
        public async Task CheckMessageAsync(VolteContext ctx)
        {
            var config = VolteBot.GetRequiredService<DatabaseService>().GetConfig(ctx.Guild);
            if (!config.ModerationOptions.MassPingChecks || UserUtil.IsAdmin(ctx)) return;
            var content = ctx.Message.Content;
            if (content.ContainsIgnoreCase("@everyone") ||
                content.ContainsIgnoreCase("@here") ||
                ctx.Message.MentionedUsers.Count > 10)
                await ctx.Message.DeleteAsync();
        }
    }
}