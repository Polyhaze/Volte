using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Volte.Discord;
using Volte.Extensions;

namespace Volte.Services
{
    public sealed class AutoroleService : IService
    {
        internal async Task ApplyRoleAsync(IGuildUser user)
        {
            var config = VolteBot.GetRequiredService<DatabaseService>().GetConfig(user.Guild.Id);
            if (!config.Autorole.IsNullOrWhitespace())
            {
                var targetRole = user.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(config.Autorole));
                await user.AddRoleAsync(targetRole);
            }
        }
    }
}