using System;

namespace Volte.Core.Entities
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RequiredArgumentAttribute : Attribute
    {
        
        public string ValidFormat { get; }

        public RequiredArgumentAttribute()
        {
            ValidFormat = null;
        }

        public RequiredArgumentAttribute(string argumentFormat)
        {
            ValidFormat = argumentFormat;
        }
    }
}