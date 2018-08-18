using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SIVA.Core.Runtime;

namespace SIVA.Plugins {
    public static class PluginRegistry<T> {

        private const string PluginsDir = "data/plugins/";
        
        static void Init() {
            if (!Directory.Exists(PluginsDir)) {
                Directory.CreateDirectory(PluginsDir);
            }
        }

        public static void LoadPlugins() {
            Init();

            var dllFileNames = Directory.GetFiles(PluginsDir, "*.dll");

            var assemblies = new List<Assembly>(dllFileNames.Length);
            assemblies.AddRange(dllFileNames.Select(dllFile => Assembly.Load(AssemblyName.GetAssemblyName(dllFile))));

            var pluginType = typeof(T);
            var pluginTypes = new List<Type>();
            foreach (var assembly in assemblies) {
                if (assembly == null) continue;

                var types = assembly.GetTypes();

                foreach (var type in types) {
                    if (type.IsInterface || type.IsAbstract) { } else {
                        if (type.GetInterface(pluginType.FullName) != null) {
                            pluginTypes.Add(type);
                        }
                    }
                }
                
                Assembly.LoadFrom(PluginsDir + assembly.GetName().Name + ".dll");
                new Log().Info($"Successfully loaded the plugin \"{assembly.GetName().Name}\"");
                
            }

            var plugins = new List<T>(pluginTypes.Count);
            foreach (var type in pluginTypes) {
                var plugin = (T) Activator.CreateInstance(type);
                plugins.Add(plugin);
            }
        }
    }
}