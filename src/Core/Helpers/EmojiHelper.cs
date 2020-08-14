using Discord;
using Gommon;

namespace Volte.Core.Helpers
{
    public static class EmojiHelper
    {
        public static string Wave { get; } = "\uD83D\uDC4B";
        public static string X { get; } = "\u274C";
        public static string BallotBoxWithCheck { get; } = "\u2611";
        public static string Repeat { get; } = "\uD83D\uDD01";
        public static string Clap { get; } = "\uD83D\uDC4F";
        public static string OkHand { get; } = "\uD83D\uDC4C";
        public static string One { get; } = "\u0031\u20E3";
        public static string Two { get; } = "\u0032\u20E3";
        public static string Three { get; } = "\u0033\u20E3";
        public static string Four { get; } = "\u0034\u20E3";
        public static string Five { get; } = "\u0035\u20E3";

        public static (Emoji One, Emoji Two, Emoji Three, Emoji Four, Emoji Five) GetPollEmojis()
            => (One.ToEmoji(), Two.ToEmoji(), Three.ToEmoji(), Four.ToEmoji(), Five.ToEmoji());

        public static (Emoji X, Emoji BallotBoxWithCheck) GetCommandEmojis() 
            => (X.ToEmoji(), BallotBoxWithCheck.ToEmoji());
    }
}