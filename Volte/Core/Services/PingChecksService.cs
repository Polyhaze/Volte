using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Discord;
using Volte.Core.Commands;
using Volte.Core.Extensions;
using Volte.Helpers;

namespace Volte.Core.Services {
    public class PingChecksService {
        public async Task CheckMessage(VolteContext ctx) {
            var config = VolteBot.ServiceProvider.GetRequiredService<DatabaseService>().GetConfig(ctx.Guild);
            if (!config.MassPingChecks || UserUtils.IsAdmin(ctx)) return;
            var content = ctx.Message.Content;
            if (content.ContainsIgnoreCase("@everyone") ||
                content.ContainsIgnoreCase("@here") ||
                ctx.Message.MentionedUserIds.Count > 10) {
                await ctx.Message.DeleteAsync();
            }
        }
    }
}