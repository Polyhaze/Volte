using System;

namespace Volte.Commands.TypeParsers
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class VolteTypeParserAttribute : Attribute
    {
        public bool OverridePrimitive { get; }

        public VolteTypeParserAttribute(bool overridePrimitive = false) 
            => OverridePrimitive = overridePrimitive;

    }
}
