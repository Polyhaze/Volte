namespace Volte
{
    public static class Version
    {
        public static int Major { get; } = 2;
        public static int Minor { get; } = 3;
        public static int Patch { get; } = 0;
        public static int Hotfix { get; } = 0;
        public static ReleaseType ReleaseType { get; } = ReleaseType.Release;
        public static string FullVersion { get; } = $"{Major}.{Minor}.{Patch}.{Hotfix}-{ReleaseType}";
    }

    public enum ReleaseType
    {
        Development,
        Release
    }
}