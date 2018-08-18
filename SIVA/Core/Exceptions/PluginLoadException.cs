using System;

namespace SIVA.Core.Exceptions {
    public class PluginLoadException : Exception {
        public PluginLoadException(string assemblyName) : base($"The plugin {assemblyName} failed to load properly.") { }
    }
}