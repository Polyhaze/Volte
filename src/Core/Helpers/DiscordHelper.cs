using System;
using Discord;
using Gommon;

namespace Volte.Core.Helpers
{
    public static class DiscordHelper
    {
        public static string Wave => "\uD83D\uDC4B";
        public static string X => "\u274C";
        public static string BallotBoxWithCheck => "\u2611";
        public static string Clap => "\uD83D\uDC4F";
        public static string OkHand => "\uD83D\uDC4C";
        public static string One => "\u0031\u20E3";
        public static string Two => "\u0032\u20E3";
        public static string Three => "\u0033\u20E3";
        public static string Four => "\u0034\u20E3";
        public static string Five => "\u0035\u20E3";
        public static string First => "⏮";
        public static string Left => "◀";
        public static string Right => "▶";
        public static string Last => "⏭";
        public static string Stop => "⏹";
        public static string E1234 => "🔢";
        public static string Question => "\u2753";

        public static (Emoji One, Emoji Two, Emoji Three, Emoji Four, Emoji Five) GetPollEmojis()
            => (One.ToEmoji(), Two.ToEmoji(), Three.ToEmoji(), Four.ToEmoji(), Five.ToEmoji());

        public static (Emoji X, Emoji BallotBoxWithCheck) GetCommandEmojis() 
            => (X.ToEmoji(), BallotBoxWithCheck.ToEmoji());

        public static RequestOptions CreateRequestOptions(Action<RequestOptions> initializer)
        {
            var opts = RequestOptions.Default;
            initializer(opts);
            return opts;
        }
    }
}