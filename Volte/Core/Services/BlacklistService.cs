using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Discord;
using Volte.Core.Data;
using Volte.Core.Extensions;
using Volte.Core.Modules;

namespace Volte.Core.Services {
    internal class BlacklistService {
        internal async Task CheckMessage(VolteContext ctx) {
            var config = VolteBot.ServiceProvider.GetRequiredService<DatabaseService>().GetConfig(ctx.Guild.Id);

            foreach (var word in config.Blacklist) {
                if (ctx.Message.Content.ContainsIgnoreCase(word)) {
                    await ctx.Message.DeleteAsync();
                    return;
                }
            }
        }
    }
}