using SysVer = System.Version;

namespace Volte
{
    public static class Version
    {
        public static SysVer AsDotNetVersion() => new SysVer(Major, Minor, Patch, Hotfix);
        private static int Major => 3;
        private static int Minor => 1;
        private static int Patch => 1;
        private static int Hotfix => 0;
        public static ReleaseType ReleaseType => ReleaseType.Release;
        public static string FullVersion => $"{Major}.{Minor}.{Patch}.{Hotfix}-{ReleaseType}";
        public static string DiscordNetVersion => Discord.DiscordConfig.Version;
    }

    public enum ReleaseType
    {
        Development,
        Release
    }
}