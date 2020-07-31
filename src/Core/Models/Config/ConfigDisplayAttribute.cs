using System;

namespace BrackeysBot
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ConfigDisplayAttribute : Attribute
    {
        public enum Mode
        {
            UserId,
            ChannelId,
            RoleId
        }

        public string Format { get; }

        public ConfigDisplayAttribute(string format)
        {
            Format = format;
        }
        public ConfigDisplayAttribute(Mode mode)
        {
            Format = mode switch
            {
                Mode.UserId => "<@{0}>",
                Mode.ChannelId => "<#{0}>",
                Mode.RoleId => "<@&{0}>",
                _ => throw new ArgumentException("mode"),
            };
        }

        public string FormatValue(params string[] args)
            => string.Format(Format, args);
    }
}