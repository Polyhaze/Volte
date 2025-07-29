namespace Volte.Systems.Interactive;

public class PaginatedAppearanceOptions
{
    public static PaginatedAppearanceOptions New => new();

    public readonly IEmote First = Emojis.TrackLast;
    public readonly IEmote Back = Emojis.ArrowLeft;
    public readonly IEmote Next = Emojis.ArrowRight;
    public readonly IEmote Last = Emojis.TrackNext;
    public readonly IEmote Stop = Emojis.WhiteLargeSquare;
    public readonly IEmote Jump = Emojis.OneTwoThreeFour;
    public readonly IEmote Info = Emojis.Question;

    public readonly string FooterFormat = "Page {0} / {1}";

    public string GenerateFooter(int currentPage, int totalPages) 
        => FooterFormat.Format(currentPage, totalPages);
    public readonly string InformationText = "This is a paginator. React with the various icons to change page and more.";

    public readonly JumpDisplayOptions JumpDisplayOptions = JumpDisplayOptions.Always;
    public readonly bool DisplayInformationIcon = true;

    public TimeSpan InfoTimeout = 30.Seconds();

    public int FieldsPerPage = 6;
}

public enum JumpDisplayOptions : uint
{
    Never = 0,
    WithManageMessages = 1,
    Always = 2
}