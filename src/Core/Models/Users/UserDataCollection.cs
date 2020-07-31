using System.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BrackeysBot
{
    public class UserDataCollection
    {
        [JsonPropertyName("users")]
        public List<UserData> Users { get; set; } = new List<UserData>();

        public bool HasUser(ulong id)
            => Users.Any(u => u.ID == id);
        public UserData GetUser(ulong id)
            => Users.FirstOrDefault(u => u.ID == id);
        public bool TryGetUser(ulong id, out UserData data)
        {
            bool hasUser = HasUser(id);
            data = hasUser ? GetUser(id) : null;
            return hasUser;
        }
        public UserData CreateUser(ulong id)
        {
            UserData data = new UserData(id);
            Users.Add(data);
            return data;
        }
        public UserData GetOrCreate(ulong id)
            => GetUser(id) ?? CreateUser(id);

        public IEnumerable<UserData> GetUsersWithTemporaryInfractions()
            => Users.Where(u => (u.TemporaryInfractions?.Count ?? 0) > 0);
    }
}
