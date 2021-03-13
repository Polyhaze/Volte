using System;

namespace Volte.Core.Entities
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class VolteTypeParserAttribute : Attribute
    {
        public bool OverridePrimitive { get; }

        public VolteTypeParserAttribute(bool overridePrimitive = false) 
            => OverridePrimitive = overridePrimitive;

    }
}
