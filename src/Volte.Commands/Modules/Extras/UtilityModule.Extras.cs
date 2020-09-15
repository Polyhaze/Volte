using System.Collections.Generic;
using System.Linq;
using DSharpPlus;
using DSharpPlus.Entities;
using Gommon;
using Humanizer;
using Volte.Core.Entities;
using Volte.Services;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule 
    {
        public CommandsService CommandsService { get; set; }
        public HttpService HttpService { get; set; }
        
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

        private (IOrderedEnumerable<(string Name, bool Value)> Allowed, IOrderedEnumerable<(string Name, bool Value)> Disallowed) GetPermissions(
            DiscordMember user)
        {
            var guildPermissions = user.GetGuildPermissions();
            var propDict = guildPermissions.GetType().GetProperties()
                .Where(a => a.PropertyType.Inherits<bool>())
                .Select(a => (a.Name.Humanize(), a.GetValue(guildPermissions).Cast<bool>()))
                .OrderByDescending(ab => ab.Item2 ? 1 : 0)
                .ToList(); //holy reflection

            return (propDict.Where(ab => ab.Item2).OrderBy(a => a.Item1), propDict.Where(ab => !ab.Item2).OrderBy(a => a.Item2));

        }

        private bool CanSeeChannel(DiscordMember member, DiscordChannel channel) 
            => member.PermissionsIn(channel).HasPermission(Permissions.AccessChannels);

        private PollInfo GetPollBody(IEnumerable<string> choices)
        {
            var c = choices as string[] ?? choices.ToArray();
            return PollInfo.FromDefaultFields(c.Length - 1, c);
        }
    }
}