using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiscordBot.Core.JSONFiles
{
    public class AutoRoles
    {
        static AutoRoles()
        {
            try
            {
                string jsonText = File.ReadAllText(filePath);
                Arconfig = JsonConvert.DeserializeObject<List<AutoRole>>(jsonText);
            }
            catch(Exception)
            {
                SaveAutoroleConfig();
            }
        }

        private static readonly List<AutoRole> Arconfig = new List<AutoRole>();
        private static string filePath = "Info/autorole.json";

        public static AutoRole GetAutoroleConfig(ulong id)
        {
            var result = from a in Arconfig
                         where a.serverId == id
                         select a;

            var config = result.FirstOrDefault();
            return config;
        }

        public static AutoRole GetOrCreateAutoroleConfig(ulong id, string role)
        {
            var result = from a in Arconfig
                         where a.serverId == id
                         select a;

            var account = result.FirstOrDefault() ?? CreateAutoroleConfig(id, role);
            return account;
        }

        public static void SaveAutoroleConfig()
        {
            var json = JsonConvert.SerializeObject(Arconfig, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static AutoRole CreateAutoroleConfig(ulong id, string role)
        {
            var newConfig = new AutoRole()
            {
                serverId = id,
                roleToApply = role
            };
            Arconfig.Add(newConfig);
            SaveAutoroleConfig();
            return newConfig;

        }


    }
}
