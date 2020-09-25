using System;

namespace Volte.Core.Entities
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class HiddenAttribute : Attribute
    {
        public HiddenAttribute()
        {
            
        }
    }
}