namespace Volte
{
    public struct Version
    {
        private static uint Major => 3;
        private static uint Minor => 0;
        private static uint Patch => 2;
        private static uint Hotfix => 0;
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