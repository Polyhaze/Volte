using System.Threading.Tasks;
using Volte.Commands;
using Volte.Discord;
using Volte.Extensions;

namespace Volte.Services
{
    public sealed class BlacklistService : IService
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