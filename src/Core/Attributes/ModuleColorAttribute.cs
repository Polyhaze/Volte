using System;

using Discord;

namespace BrackeysBot
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ModuleColorAttribute : Attribute
    {
        public Color Color { get; }

        public ModuleColorAttribute(uint rawValue)
        {
            Color = new Color(rawValue);
        }
    }
}
