using System;

namespace Volte.Core.Entities
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class OptionalArgumentAttribute : Attribute
    {
        public string ValidFormat { get; }

        public OptionalArgumentAttribute()
        {
            ValidFormat = null;
        }

        public OptionalArgumentAttribute(string argumentFormat)
        {
            ValidFormat = argumentFormat;
        }
    }
}