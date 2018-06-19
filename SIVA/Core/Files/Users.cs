using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using SIVA.Core.Runtime;

namespace SIVA.Core.Files
{
    public class DiscordUser
    {
        public string Tag { get; internal set; }
        public ulong Id { get; internal set; }
        public ulong Xp { get; set; }
        public uint Level => (uint)Math.Sqrt(Xp/ 50);
        public int Money { get; set; }    
    }

    public static class Users
    {
        private static readonly List<DiscordUser> users;
        private static readonly string usersFile = "data/users.json";

        static Users()
        {
            if (File.Exists(usersFile))
            {
                users = Load();   
            }
            else
            {
                users = new List<DiscordUser>();
                Save();
            }
        }
        
        public static void Save()
        {
            File.WriteAllText(usersFile, JsonConvert.SerializeObject(users, Formatting.Indented));
        }

        public static List<DiscordUser> Load()
        {
            if (!File.Exists(usersFile))
                File.Create(usersFile);

            var json = File.ReadAllText(usersFile);
            return JsonConvert.DeserializeObject<List<DiscordUser>>(json);
        }

        public static DiscordUser Get(ulong id)
        {
            var res = users.FirstOrDefault(x => x.Id == id) ?? Create(id);
            return res;
        }

        public static DiscordUser Create(ulong id)
        {
            var newUser = new DiscordUser
            {
                Tag = $"{Program.Client.GetUser(id).Username}#{Program.Client.GetUser(id).Discriminator}",
                Id = id,
                Xp = 5,
                Money = 0
            };
            users.Add(newUser);
            Save();
            return newUser;
        }

    }
    
}