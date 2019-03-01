using System.Threading.Tasks;
using Volte.Core.Commands;
using Volte.Core.Discord;
using Volte.Core.Extensions;

namespace Volte.Core.Services
{
    internal class BlacklistService : IService
    {
        internal async Task CheckMessageAsync(VolteContext ctx)
        {
            var config = VolteBot.GetRequiredService<DatabaseService>().GetConfig(ctx.Guild.Id);

            foreach (var word in config.ModerationOptions.Blacklist)
                if (ctx.Message.Content.ContainsIgnoreCase(word))
                {
                    await ctx.Message.DeleteAsync();
                    return;
                }
        }
    }
}