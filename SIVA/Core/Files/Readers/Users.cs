using SIVA.Core.Files.Objects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SIVA.Core.Discord;
using SIVA.Core.Runtime;

namespace SIVA.Core.Files.Readers {
    public static class Users {
        private static readonly List<DiscordUser> LoadedUsers;
        private const string UsersFile = "data/users.json";

        static Users() {
            if (File.Exists(UsersFile)) {
                LoadedUsers = Load();
            }
            else {
                LoadedUsers = new List<DiscordUser>();
                Save();
            }
        }

        public static void Save() {
            File.WriteAllText(UsersFile, JsonConvert.SerializeObject(LoadedUsers, Formatting.Indented));
        }

        public static List<DiscordUser> Load() {
            if (!File.Exists(UsersFile))
                File.Create(UsersFile);

            var json = File.ReadAllText(UsersFile);
            return JsonConvert.DeserializeObject<List<DiscordUser>>(json);
        }

        public static DiscordUser Get(ulong id) {
            return LoadedUsers.FirstOrDefault(x => x.Id == id) ?? Create(id);
        }

        public static DiscordUser Create(ulong id) {
            var newUser = new DiscordUser {
                Tag = $"{Discord.Siva.GetInstance().GetUser(id).Username}#{Discord.Siva.GetInstance().GetUser(id).Discriminator}",
                Id = id,
                Xp = 5,
                Money = 0
            };
            LoadedUsers.Add(newUser);
            Save();
            return newUser;
        }
    }
}