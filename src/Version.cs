namespace Volte
{
    public static class Version
    {
        internal static int Major { get; } = 2;
        internal static int Minor { get; } = 3;
        internal static int Patch { get; } = 1;
        internal static int Hotfix { get; } = 0;
        public static ReleaseType ReleaseType { get; } = ReleaseType.Release;
        public static string FullVersion { get; } = $"{Major}.{Minor}.{Patch}.{Hotfix}-{ReleaseType}";
    }

    public enum ReleaseType
    {
        Development,
        Release
    }
}