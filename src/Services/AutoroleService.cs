using System.Threading.Tasks;
using DSharpPlus.Entities;
using Volte.Extensions;
using System.Linq;
using DSharpPlus.EventArgs;

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

        internal async Task ApplyRoleAsync(GuildMemberAddEventArgs args)
        {
            var config = _db.GetConfig(args.Guild.Id);
            if (!config.Autorole.IsNullOrWhitespace())
            {
                var targetRole = args.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(config.Autorole));
                await args.Member.GrantRoleAsync(targetRole);
            }
        }
    }
}