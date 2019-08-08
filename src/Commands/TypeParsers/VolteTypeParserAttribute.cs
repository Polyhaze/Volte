using System;
using System.Collections.Generic;
using System.Text;

namespace Volte.Commands.TypeParsers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VolteTypeParserAttribute : Attribute
    {

        public bool OverridePrimitive { get; set; }

        public VolteTypeParserAttribute(bool overridePrimitive = false)
        {
            OverridePrimitive = overridePrimitive;
        }

    }
}
