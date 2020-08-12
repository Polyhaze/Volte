using System.Collections.Generic;
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
        private readonly (char Key, string Value)[] _nato = new List<(char Key, string Value)>
        {
            ('a', "Alfa"), ('b', "Bravo"), ('c', "Charlie"), ('d', "Delta"),
            ('e', "Echo"), ('f', "Foxtrot"), ('g', "Golf"), ('h', "Hotel"),
            ('i', "India"), ('j', "Juliett"), ('k', "Kilo"), ('l', "Lima"),
            ('m', "Mike"), ('n', "November"), ('o', "Oscar"), ('p', "Papa"),
            ('q', "Quebec"), ('r', "Romeo"), ('s', "Sierra"), ('t', "Tango"),
            ('u', "Uniform"), ('v', "Victor"), ('w', "Whiskey"), ('x', "X-ray"),
            ('y', "Yankee"), ('z', "Zulu"), ('1', "One"), ('2', "Two"),
            ('3', "Three"), ('4', "Four"), ('5', "Five"), ('6', "Six"), 
            ('7', "Seven"), ('8', "Eight"), ('9', "Nine"), ('0', "Zero")

        }.ToArray();

        private string GetNato(char i) => 
            _nato.First(x => x.Key.ToString().EqualsIgnoreCase(i.ToString())).Value;

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