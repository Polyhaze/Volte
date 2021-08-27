using SysVer = System.Version;

namespace Volte
{
    public static class Version
    {
        public enum DevelopmentStage
        {
            Development,
            Release
        }
        
        public static bool IsDevelopment => ReleaseType is DevelopmentStage.Development;
#if DEBUG
        public static DevelopmentStage ReleaseType => DevelopmentStage.Development;
#else
        public static DevelopmentStage ReleaseType => DevelopmentStage.Release;
#endif
        public static string FullVersion => $"{Major}.{Minor}.{Patch}.{Hotfix}-{ReleaseType}";
        public static string DiscordNetVersion => Discord.DiscordConfig.Version;
        public static SysVer AsDotNetVersion() => new SysVer(Major, Minor, Patch, Hotfix);
        
        private static int Major => 5;
        private static int Minor => 0;
        private static int Patch => 0;
        private static int Hotfix => 0;
    }
}
