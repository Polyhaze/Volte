using Discord;

namespace Volte
{
    public static class Version
    {
        private static uint Major => 3;
        private static uint Minor => 0;
        private static uint Patch => 0;
        private static uint Hotfix => 0;
        public static ReleaseType ReleaseType => ReleaseType.Development;
        public static string FullVersion => $"{Major}.{Minor}.{Patch}.{Hotfix}-{ReleaseType}";
        public static string DiscordNetVersion => DiscordConfig.Version;
    }

    public enum ReleaseType
    {
        Development,
        Release
    }
}