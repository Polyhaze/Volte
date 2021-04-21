using System;
using Discord;
using Humanizer;
using Volte.Core.Helpers;

namespace Volte.Interactive
{
    public class PaginatedAppearanceOptions
    {
        public static PaginatedAppearanceOptions New => new PaginatedAppearanceOptions();

        public readonly IEmote First = DiscordHelper.First.ToEmoji();
        public readonly IEmote Back = DiscordHelper.Left.ToEmoji();
        public readonly IEmote Next = DiscordHelper.Right.ToEmoji();
        public readonly IEmote Last = DiscordHelper.Last.ToEmoji();
        public readonly IEmote Stop = DiscordHelper.WhiteSquare.ToEmoji();
        public readonly IEmote Jump = DiscordHelper.E1234.ToEmoji();
        public readonly IEmote Info = DiscordHelper.Question.ToEmoji();

        public string FooterFormat => "Page {0} / {1}";

        public string GenerateFooter(int currentPage, int totalPages) 
            => FooterFormat.FormatWith(currentPage, totalPages); 
        public string InformationText => "This is a paginator. React with the various icons to change page and more.";

        public JumpDisplayOptions JumpDisplayOptions => JumpDisplayOptions.Always;
        public bool DisplayInformationIcon => true;
        
        public TimeSpan InfoTimeout = 30.Seconds();

        public int FieldsPerPage => 6;
    }

    public enum JumpDisplayOptions : uint
    {
        Never = 0,
        WithManageMessages = 1,
        Always = 2
    }
}
