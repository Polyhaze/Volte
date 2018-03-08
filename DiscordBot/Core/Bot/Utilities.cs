using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using Discord.Commands;

namespace SIVA.Core.Bot
{
    class Utilities
    {
        private static Dictionary<string, string> alerts; 

        static Utilities()
        {
            string json = File.ReadAllText("Resources/Locale.json");
            var data = JsonConvert.DeserializeObject<dynamic>(json);
            alerts = data.ToObject<Dictionary<string, string>>();
        }

        public static string GetLocaleMsg(string key)
        {
            if (alerts.ContainsKey(key)) return alerts[key];
            return "";
        }

        public static string GetFormattedLocaleMsg(string key, params object[] parameter)
        {
            if (alerts.ContainsKey(key))
            {
                return String.Format(alerts[key], parameter);
            }
            return "";
        }

        public static string GetFormattedLocaleMsg(string key, object parameter)
        {
            return GetFormattedLocaleMsg(key, new object[] { parameter });
        }

    }
}
