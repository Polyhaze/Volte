using System.Collections.Generic;
using System.IO;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace SIVA.Core.Bot.Internal
{
    internal class Utilities
    {
        private static readonly Dictionary<string, string> alerts;

        static Utilities()
        {
            var json = File.ReadAllText("data/Locale.json");
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
            return alerts.ContainsKey(key) ? string.Format(alerts[key], parameter) : "";
        }

        public static string GetFormattedLocaleMsg(string key, object parameter)
        {
            return alerts.ContainsKey(key) ? string.Format(alerts[key], parameter) : "";
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