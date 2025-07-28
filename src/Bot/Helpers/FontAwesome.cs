// ReSharper disable MemberCanBePrivate.Global


namespace Volte.Helpers;

using static IconVariant;

public enum IconVariant { Solid, Regular, Light, Thin }

public static class FontAwesome
{
    public static readonly FontAwesomeIcon Check = Solid.Named("check");
    public static readonly FontAwesomeIcon Message = Regular.Named("message");
    public static readonly FontAwesomeIcon RightToBracket = Regular.Named("right-to-bracket");
    public static readonly FontAwesomeIcon Brush = Solid.Named("brush");
    public static readonly FontAwesomeIcon Copy = Regular.Named("copy");
    
    public static FontAwesomeIcon Named(this IconVariant defaultVariant, string name) => new(name, defaultVariant);
}

public readonly record struct FontAwesomeIcon
{
    public FontAwesomeIcon() : this(string.Empty, (IconVariant)int.MaxValue) {}
        
    public FontAwesomeIcon(string name, IconVariant defaultVariant)
    {
        Name = name;
        DefaultVariant = defaultVariant;
    }
        
    public readonly string Name;
    public readonly IconVariant DefaultVariant;
        
    public string Format(IconVariant variant = IconVariant.Regular) => $"fa-{Enum.GetName(variant)?.ToLower() ?? "regular"} fa-{Name}";
        
    public string Solid => Format(IconVariant.Solid);
    public string Regular => Format();
    public string Light => Format(IconVariant.Light);
    public string Thin => Format(IconVariant.Thin);

    public override string ToString() => Format(DefaultVariant);
    public static implicit operator string(FontAwesomeIcon icon) => icon.ToString();
}

