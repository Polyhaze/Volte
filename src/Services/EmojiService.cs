using Discord;
using Gommon;

namespace Volte.Services
{
    public sealed class EmojiService : VolteService
    {
        public string Wave { get; } = "\uD83D\uDC4B";
        public string X { get; } = "\u274C";
        public string BallotBoxWithCheck { get; } = "\u2611";
        public string Repeat { get; } = "\uD83D\uDD01";
        public string Clap { get; } = "\uD83D\uDC4F";
        public string OkHand { get; } = "\uD83D\uDC4C";
        public string One { get; } = "\u0031\u20E3";
        public string Two { get; } = "\u0032\u20E3";
        public string Three { get; } = "\u0033\u20E3";
        public string Four { get; } = "\u0034\u20E3";
        public string Five { get; } = "\u0035\u20E3";

        public (Emoji One, Emoji Two, Emoji Three, Emoji Four, Emoji Five) GetPollEmojis()
            => (One.ToEmoji(), Two.ToEmoji(), Three.ToEmoji(), Four.ToEmoji(), Five.ToEmoji());

        public (Emoji X, Emoji BallotBoxWithCheck) GetCommandEmojis() 
            => (X.ToEmoji(), BallotBoxWithCheck.ToEmoji());
    }
}