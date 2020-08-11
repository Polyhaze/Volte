using System.Linq;
using Discord;
using Discord.WebSocket;
using Gommon;
using Humanizer;
using Volte.Services;
// ReSharper disable MemberCanBePrivate.Global

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule : VolteModule
    {
        private readonly string _baseWikiUrl = "https://github.com/Ultz/Volte/wiki";
        public CommandsService CommandsService { get; set; }
        
        private (IOrderedEnumerable<(string Name, bool Value)> Allowed, IOrderedEnumerable<(string Name, bool Value)> Disallowed) GetPermissions(
            SocketGuildUser user)
        {
            var propDict = user.GuildPermissions.GetType().GetProperties()
                .Where(a => a.PropertyType.Inherits<bool>())
                .Select(a => (a.Name.Humanize(), a.GetValue(user.GuildPermissions).Cast<bool>()))
                .OrderByDescending(ab => ab.Item2 ? 1 : 0)
                .ToList(); //holy reflection

            return (propDict.Where(ab => ab.Item2).OrderBy(a => a.Item1), propDict.Where(ab => !ab.Item2).OrderBy(a => a.Item2));

        }

        private bool CanSeeChannel(IGuildUser member, IGuildChannel channel)
        {
            return member.GetPermissions(channel).Connect || member.GetPermissions(channel).ViewChannel;
        }

    }
}