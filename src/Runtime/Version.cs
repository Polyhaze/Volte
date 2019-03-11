namespace Volte.Runtime
{
    public static class Version
    {
        public static int Major { get; } = 2;
        public static int Minor { get; } = 2;
        public static int Patch { get; } = 1;
        public static int Hotfix { get; } = 0;
        public static ReleaseType ReleaseType { get; } = ReleaseType.Development;
        public static string FullVersion { get; } = $"{Major}.{Minor}.{Patch}.{Hotfix}-{ReleaseType}";
    }

    public enum ReleaseType
    {
        Development,
        Release
    }
}