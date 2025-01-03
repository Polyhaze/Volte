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
        
        var infoVer = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? throw new InvalidOperationException("Version not found");

        infoVer = infoVer.LastIndexOf('+') != -1
            ? infoVer[..infoVer.LastIndexOf('+')]
            : infoVer;

        InformationVersion = $"{DotNetVersion} {infoVer.Trim()}";
    }

    public static readonly bool IsDevelopment = InformationVersion.ContainsIgnoreCase("dev");
    
    public static string DiscordNetVersion => DiscordConfig.Version;
    
    public static int Major => DotNetVersion.Major;
    public static int Minor => DotNetVersion.Minor;
    public static int Patch => DotNetVersion.Build;
    public static int Hotfix => DotNetVersion.Revision;
}