using System;

namespace BrackeysBot
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class DescriptionAttribute : Attribute
    {
        public string Description => _description;
        private readonly string _description;

        public DescriptionAttribute(string description)
        {
            _description = description;
        }
    }
}