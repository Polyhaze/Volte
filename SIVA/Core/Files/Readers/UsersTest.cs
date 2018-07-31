using System.IO;
using Discord.WebSocket;
using Newtonsoft.Json;
using SIVA.Core.Files.Objects;

namespace SIVA.Core.Files.Readers {
    public static class UsersTest {

        public static DiscordUser Get(SocketUser user) {
            return !File.Exists($"data/users/{user.Id}.json") ? Create(user) : JsonConvert.DeserializeObject<DiscordUser>(File.ReadAllText($"data/users/{user.Id}.json"));
        }

        public static DiscordUser Create(SocketUser user) {
            File.Create($"data/users/{user.Id}.json");
            var fileContent = new DiscordUser {
                Id = user.Id,
                Tag = $"{user.Username}#{user.Discriminator}"
            };
            
            File.WriteAllText($"data/users/{user.Id}.json", JsonConvert.SerializeObject(fileContent, Formatting.Indented));
            return fileContent;
        }

    }
}