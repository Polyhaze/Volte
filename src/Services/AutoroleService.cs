using System.Linq;
using System.Threading.Tasks;
using Discord;
using Volte.Data.Objects.EventArgs;
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

        internal async Task ApplyRoleAsync(UserJoinedEventArgs args)
        {
            if (!args.Config.Autorole.IsNullOrWhitespace())
            {
                var targetRole = args.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(args.Config.Autorole));
                await args.User.AddRoleAsync(targetRole);
            }
        }
    }
}