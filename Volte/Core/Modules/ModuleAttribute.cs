using System;

namespace Volte.Core.Modules {
    [AttributeUsage(AttributeTargets.Method)]
    public class ModuleAttribute : Attribute {
       public string Name { get; }

       public ModuleAttribute(string text) {
           Name = text;
       }
    }
}