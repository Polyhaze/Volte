using Discord;

namespace Volte
{
    public static class Version
    {
        internal static int Major { get; } = 3;
        internal static int Minor { get; } = 0;
        internal static int Patch { get; } = 0;
        internal static int Hotfix { get; } = 0;
        public static ReleaseType ReleaseType { get; } = ReleaseType.Development;
        public static string FullVersion { get; } = $"{Major}.{Minor}.{Patch}.{Hotfix}-{ReleaseType}";
        public static string DiscordNetVersion { get; } = DiscordConfig.Version;
    }

    public enum ReleaseType
    {
        Development,
        Release
    }
}