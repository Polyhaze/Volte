using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Discord;
using Volte.Core.Extensions;
using Volte.Core.Modules;
using Volte.Helpers;

namespace Volte.Core.Services {
    public class PingChecksService {
        public async Task CheckMessage(VolteContext ctx) {
            var content = ctx.Message.Content;
            var config = VolteBot.ServiceProvider.GetRequiredService<DatabaseService>().GetConfig(ctx.Guild);
            if (!config.MassPingChecks || UserUtils.IsAdmin(ctx)) return;
            if (content.ContainsIgnoreCase("@everyone") ||
                content.ContainsIgnoreCase("@here") ||
                ctx.Message.MentionedUserIds.Count > 10) {
                await ctx.Message.DeleteAsync();
            }
        }
    }
}