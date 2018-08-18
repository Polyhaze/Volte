namespace SIVA.Core.Runtime {
    public static class Version {
        private static readonly int Major = 2;
        private static readonly int Minor = 0;
        private static readonly int Patch = 0;
        private static readonly int Hotfix = 0;
        private static readonly ReleaseType ReleaseType = ReleaseType.Development;

        public static string GetFullVersion() {
            return $"{Major}.{Minor}.{Patch}.{Hotfix}-{ReleaseType}";
        }

        public static int GetMajorVersion() => Major;

        public static int GetMinorVersion() => Minor;

        public static int GetPatchVersion() => Patch;

        public static int GetHotfixVersion() => Hotfix;

        public static ReleaseType GetReleaseType() => ReleaseType;
    }

    public enum ReleaseType {
        Development,
        Alpha,
        Beta,
        Prerelease,
        Release
    }
}