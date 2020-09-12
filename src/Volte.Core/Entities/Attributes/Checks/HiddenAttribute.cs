using System;

namespace Volte.Core.Entities
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HiddenAttribute : Attribute
    {
        public HiddenAttribute()
        {
            
        }
    }
}