namespace Volte
{
    public static class Version
    {
        public static System.Version AsDotNetVersion() => new System.Version(Major, Minor, Patch, Hotfix);
        private static int Major => 3;
        private static int Minor => 1;
        private static int Patch => 0;
        private static int Hotfix => 0;
        public static ReleaseType ReleaseType => ReleaseType.Development;
        public static string FullVersion => $"{Major}.{Minor}.{Patch}.{Hotfix}-{ReleaseType}";
        public static string DiscordNetVersion => Discord.DiscordConfig.Version;
    }

    public enum ReleaseType
    {
        Development,
        Release
    }
}