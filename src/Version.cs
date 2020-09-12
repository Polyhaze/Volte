using System;
using DSharpPlus;
using SysVer = System.Version;

namespace Volte
{
    public static class Version
    {
        public static SysVer AsDotNetVersion() => new SysVer(Major, Minor, Patch, Hotfix);
        private static int Major => 4;
        private static int Minor => 0;
        private static int Patch => 0;
        private static int Hotfix => 0;
        public static DevelopmentStage ReleaseType => DevelopmentStage.Development;
        public static string FullVersion => $"{Major}.{Minor}.{Patch}.{Hotfix}-{ReleaseType}";
        private static readonly Lazy<string> VersionString = new Lazy<string>(() =>
        {
            using var client = new DiscordClient(new DiscordConfiguration());
            return client.VersionString;
        });
        public static string DSharpPlusVersion => VersionString.Value;
        public enum DevelopmentStage
        {
            Development,
            Release
        }
    }
}
