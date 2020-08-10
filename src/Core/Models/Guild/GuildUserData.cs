using System.Collections.Generic;

namespace Volte.Core.Models.Guild
{
    public class GuildUserData
    {
        public ulong Id { get; set; }
        public List<ModAction> Actions { get; set; }
        public string Note { get; set; }

        public long ActionCount => Actions.Count;

        public GuildUserData()
        {
            Actions = new List<ModAction>();
        }
    }
}