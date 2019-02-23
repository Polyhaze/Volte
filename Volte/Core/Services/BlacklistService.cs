using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volte.Core.Commands;
using Volte.Core.Discord;
using Volte.Core.Extensions;

namespace Volte.Core.Services
{
    internal class BlacklistService : IService
    {
        internal async Task CheckMessageAsync(VolteContext ctx)
        {
            var config = VolteBot.ServiceProvider.GetRequiredService<DatabaseService>().GetConfig(ctx.Guild.Id);

            foreach (var word in config.Blacklist)
                if (ctx.Message.Content.ContainsIgnoreCase(word))
                {
                    await ctx.Message.DeleteAsync();
                    return;
                }
        }
    }
}