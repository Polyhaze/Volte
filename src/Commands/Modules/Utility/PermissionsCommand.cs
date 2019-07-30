
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule : VolteModule
    {
        [Command("Permissions", "Perms")]
        [Description("Shows someone's, or the command invoker's, permissions in the current guild.")]
        [Remarks("Usage: |prefix|permissions [user]")]
        public Task<ActionResult> PermissionsAsync(SocketGuildUser user = null)
        {
            user ??= Context.User; // Get the user (or the invoker, if none specified)


            if (user.Id == Context.Guild.OwnerId)
            {
                return Ok("User is owner of server, and has all permissions");
            }

            if (user.GuildPermissions.Administrator)
            {
                return Ok("User has Administrator permission, and has all permissions");
            }


            var perms = GetPermissions(user);

            var allowed = perms.Allowed.Select(a => $"- {a.Name}").Join('\n');
            var disallowed = perms.Disallowed.Select(a => $"- {a.Name}").Join('\n');
            return Ok(Context.CreateEmbedBuilder().WithAuthor(user)
                .AddField("Allowed", string.IsNullOrEmpty(allowed) ? "- None" : allowed, true)
                .AddField("Denied", string.IsNullOrEmpty(disallowed) ? "- None" : disallowed, true));
        }

        private (IOrderedEnumerable<(string Name, bool Value)> Allowed, IOrderedEnumerable<(string Name, bool Value)> Disallowed) GetPermissions(
            SocketGuildUser user)
        {
            var booleanTypeProperties = user.GuildPermissions.GetType().GetProperties()
                .Where(a => a.PropertyType.IsAssignableFrom(typeof(bool)))
                .ToList();

            var propDict = booleanTypeProperties.Select(a => (a.Name.Humanize(), a.GetValue(user.GuildPermissions).Cast<bool>()))
                .OrderByDescending(ab => ab.Item2 ? 1 : 0)
                .ToList();

            var accept =
                propDict.Where(ab => ab.Item2).OrderBy(a => a.Item1);
            var deny = propDict.Where(ab => !ab.Item2).OrderBy(a => a.Item2);

            return (accept, deny);

        }
    }
}
