using System.Linq;
using System.Threading.Tasks;
using Discord;
using Volte.Extensions;

namespace Volte.Services
{
    [Service("Autorole", "The main Service for automatically applying roles when a user joins any given guild.")]
    public sealed class AutoroleService
    {
        private readonly DatabaseService _db;

        public AutoroleService(DatabaseService databaseService)
        {
            _db = databaseService;
        }

        internal async Task ApplyRoleAsync(IGuildUser user)
        {
            var config = _db.GetConfig(user.Guild.Id);
            if (!config.Autorole.IsNullOrWhitespace())
            {
                var targetRole = user.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(config.Autorole));
                await user.AddRoleAsync(targetRole);
            }
        }
    }
}