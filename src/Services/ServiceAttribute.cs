using System;
using System.Collections.Generic;
using System.Text;

namespace Volte.Services
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ServiceAttribute : Attribute
    {
        public string Name { get; }
        public string Purpose { get; }

        public ServiceAttribute(string name, string purpose)
        {
            Name = name;
            Purpose = purpose;
        }

    }
}
