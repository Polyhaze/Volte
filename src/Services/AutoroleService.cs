using System.Linq;
using System.Threading.Tasks;
using Volte.Data.Objects.EventArgs;
using Gommon;

namespace Volte.Services
{
    [Service("Autorole", "The main Service for automatically applying roles when a user joins any given guild.")]
    public sealed class AutoroleService
    {
        internal async Task ApplyRoleAsync(UserJoinedEventArgs args)
        {
            if (!args.Config.Autorole.IsNullOrWhitespace())
            {
                var targetRole = args.Guild.Roles.FirstOrDefault(r => r.Name.EqualsIgnoreCase(args.Config.Autorole));
                if (targetRole is null) return;
                await args.User.AddRoleAsync(targetRole);
            }
        }
    }
}