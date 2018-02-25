using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using Discord.Commands;

namespace SIVA
{
    class Utilities : ModuleBase<SocketCommandContext>
    {
        private static Dictionary<string, string> alerts; 

        static Utilities()
        {
            string json = File.ReadAllText("Resources/alerts.json");
            var data = JsonConvert.DeserializeObject<dynamic>(json);
            alerts = data.ToObject<Dictionary<string, string>>();
        }

        public static string GetAlert(string key)
        {
            if (alerts.ContainsKey(key)) return alerts[key];
            return "";
        }

        public static string GetFormattedAlert(string key, params object[] parameter)
        {
            if (alerts.ContainsKey(key))
            {
                return String.Format(alerts[key], parameter);
            }
            return "";
        }

        public static string GetFormattedAlert(string key, object parameter)
        {
            return GetFormattedAlert(key, new object[] { parameter });
        }

    }
}
