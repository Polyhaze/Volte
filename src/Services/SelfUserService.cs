using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

using Discord;

namespace BrackeysBot.Services
{
    public class SelfUserService : BrackeysBotService
    {
        private readonly DataService _data;

        public SelfUserService(DataService data)
        {
            _data = data;
        }

        public IRole GetTeam(IGuildUser user, IGuild guild)
            => GetTeams(guild).FirstOrDefault(team => user.RoleIds.Contains(team.Id));
        public IRole GetTeamByName(string name, IGuild guild)
            => GetTeams(guild).FirstOrDefault(team => team.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        public IEnumerable<IRole> GetTeams(IGuild guild)
            => _data.Configuration.TeamRoleIDs.Select(id => guild.GetRole(id));

        public bool IsUserRole(IRole role)
            => GetUserRoles(role.Guild).Contains(role);
        public IEnumerable<IRole> GetUserRoles(IGuild guild)
            => _data.Configuration.UserRoleIDs.Select(id => guild.GetRole(id));
    }
}
