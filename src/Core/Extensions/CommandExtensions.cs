using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BrackeysBot
{
    public static class CommandExtensions
    {
        public static bool HasAttribute<T>(this ModuleInfo info) where T : Attribute
            => info.Attributes.Any(a => a.GetType() == typeof(T));
        public static bool HasAttribute<T>(this CommandInfo info) where T : Attribute
            => info.Attributes.Any(a => a.GetType() == typeof(T));

        public static T GetAttribute<T>(this ModuleInfo info) where T : Attribute
            => info.Attributes.FirstOrDefault(a => a.GetType() == typeof(T)) as T;
        public static T GetAttribute<T>(this CommandInfo info) where T : Attribute
            => info.Attributes.FirstOrDefault(a => a.GetType() == typeof(T)) as T;

        public static Color GetColor(this ModuleInfo info)
            => info.GetAttribute<ModuleColorAttribute>()?.Color ?? Color.DarkerGrey;
        public static Color GetColor(this CommandInfo info)
            => info.Module.GetColor();
    }
}
