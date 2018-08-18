using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SIVA.Plugins {
    public static class PluginRegistry<T> {

        private const string PluginsDir = "plugins";
        
        public static void Init() {
            if (!Directory.Exists("plugins")) {
                Directory.CreateDirectory("plugins");
            }
        }

        public static void LoadPlugins() {
            string[] dllFileNames;

            Init();

            dllFileNames = Directory.GetFiles(PluginsDir, "*.dll");

            var assemblies = new List<Assembly>(dllFileNames.Length);
            foreach (var dllFile in dllFileNames) {
                assemblies.Add(Assembly.Load(AssemblyName.GetAssemblyName(dllFile)));
            }

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
            }

            var plugins = new List<T>(pluginTypes.Count);
            foreach (var type in pluginTypes) {
                var plugin = (T) Activator.CreateInstance(type);
                plugins.Add(plugin);
            }

            foreach (var plugin in plugins) {
                Assembly.LoadFrom($"{PluginsDir}/{plugin}");
            }
        }
    }
}