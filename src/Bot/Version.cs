using SysVer = System.Version;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Volte;

public static class Version
{
    public static string InformationVersion { get; }
    
    public static SysVer DotNetVersion { get; }
    
    static Version()
    {
        DotNetVersion = Assembly.GetExecutingAssembly().GetName().Version ??
                        throw new InvalidOperationException("Version not found");

        // ReSharper disable once HeuristicUnreachableCode
        //compile-time constant is determined by a define constant
        InformationVersion = $"{DotNetVersion} {(VolteBot.IsProduction ? "Release" : "indev")}";
    }

    public static readonly bool IsDevelopment = InformationVersion.ContainsIgnoreCase("dev");
    
    public static string DiscordNetVersion => DiscordConfig.Version;
    
    public static int Major => DotNetVersion.Major;
    public static int Minor => DotNetVersion.Minor;
    public static int Patch => DotNetVersion.Build;
    public static int Hotfix => DotNetVersion.Revision;
}