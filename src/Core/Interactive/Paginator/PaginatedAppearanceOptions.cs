using System;
using Discord;

namespace Volte.Interactive
{
    public class PaginatedAppearanceOptions
    {
        public static PaginatedAppearanceOptions Default = new PaginatedAppearanceOptions();

        public IEmote First = new Emoji("⏮");
        public IEmote Back = new Emoji("◀");
        public IEmote Next = new Emoji("▶");
        public IEmote Last = new Emoji("⏭");
        public IEmote Stop = new Emoji("⏹");
        public IEmote Jump = new Emoji("🔢");
        public IEmote Info = new Emoji("ℹ");

        public string FooterFormat = "Page {0}/{1}";
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
