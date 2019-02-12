using Discord.Commands;

namespace Volte.Core.Extensions {
    public static class ModuleExtensions {
        public static string SanitizeName(this ModuleInfo m) {
            return m.Name.Replace("Module", string.Empty);
        }
    }
}