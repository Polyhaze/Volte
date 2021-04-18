using System;

namespace Volte.Core.Entities
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class InjectTypeParserAttribute : Attribute
    {
        public bool OverridePrimitive { get; }

        public InjectTypeParserAttribute(bool overridePrimitive = false) 
            => OverridePrimitive = overridePrimitive;

    }
}
