namespace SIVA.Core.Runtime
{
    public static class Version
    {
        private static int Major = 2;
        private static int Minor = 0;
        private static int Patch = 0;
        private static int Hotfix = 0;
        private static string ReleaseType = "beta";

        public static string GetFullVersion()
        {
            return $"{Major}.{Minor}.{Patch}.{Hotfix}-{ReleaseType}";
        }

        public static int GetMajorVersion()
        {
            return Major;
        }

        public static int GetMinorVersion()
        {
            return Minor;
        }

        public static int GetPatchVersion()
        {
            return Patch;
        }

        public static int GetHotfixVersion()
        {
            return Hotfix;
        }

        public static string GetReleaseType()
        {
            return ReleaseType;
        }
    }
}