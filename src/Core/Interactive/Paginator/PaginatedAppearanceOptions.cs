using System;
using Discord;
using Gommon;

namespace Volte.Interactive
{
    public class PaginatedAppearanceOptions
    {
        public static PaginatedAppearanceOptions Default = new PaginatedAppearanceOptions();

        public IEmote First = "⏮".ToEmoji();
        public IEmote Back = "◀".ToEmoji();
        public IEmote Next = "▶".ToEmoji();
        public IEmote Last = "⏭".ToEmoji();
        public IEmote Stop = "⏹".ToEmoji();
        public IEmote Jump = "🔢".ToEmoji();
        public IEmote Info = "ℹ".ToEmoji();

        public string FooterFormat = "Page: {0} of {1}";
        public string InformationText = "This is a paginator. React with the respective icons to change page.";

        public JumpDisplayOptions JumpDisplayOptions = JumpDisplayOptions.WithManageMessages;
        public bool DisplayInformationIcon = true;

        public TimeSpan? Timeout = null;
        public TimeSpan InfoTimeout = TimeSpan.FromSeconds(30);

        public int FieldsPerPage = 6;
    }

    public enum JumpDisplayOptions
    {
        Never,
        WithManageMessages,
        Always
    }
}
