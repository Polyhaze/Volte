using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

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

        public static string InlineMsg(string msg)
        {
            return $"`{msg}`";
        }

        public static string CodeBlock(string msg)
        {
            return $"```{msg}```";
        }

        public static string Italic(string msg)
        {
            return $"*{msg}*";
        }

        public static string Bold(string msg)
        {
            return $"**{msg}**";
        }

        public static string BoldItalic(string msg)
        {
            return $"***{msg}***";
        }

        public static string Underline(string msg)
        {
            return $"__{msg}__";
        }

        public static string Strikethrough(string msg)
        {
            return $"~~{msg}~~";
        } 

        public static int Multiply(int a, int b)
        {
            return a * b;
        }

        public static float Divide(int a, int b)
        {
            return a / b;
        }

        public static int Add(int a, int b)
        {
            return a + b;
        }

        public static int Subtract(int a, int b)
        {
            return a - b;
        }
    }
}
