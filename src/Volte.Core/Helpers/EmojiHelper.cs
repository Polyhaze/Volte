using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using DSharpPlus.Entities;
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
        public static string Star { get; } = "\u2B50";
        public static string First { get; } = "⏮";
        public static string Back { get; } = "◀";
        public static string Next { get; } = "▶";
        public static string Last { get; } = "⏭";
        public static string Stop { get; } = "⏹";
        public static string Jump { get; } = "🔢";
        public static string Info { get; } = "ℹ";

        public static (DiscordEmoji One, DiscordEmoji Two, DiscordEmoji Three, DiscordEmoji Four, DiscordEmoji Five) GetPollEmojis()
            => (One.ToEmoji(), Two.ToEmoji(), Three.ToEmoji(), Four.ToEmoji(), Five.ToEmoji());

        public static IReadOnlyList<DiscordEmoji> GetPollEmojisList()
        {
            return new[]
            {
                One.ToEmoji(),
                Two.ToEmoji(),
                Three.ToEmoji(),
                Four.ToEmoji(),
                Five.ToEmoji()
            };
        }

        public static (DiscordEmoji X, DiscordEmoji BallotBoxWithCheck) GetCommandEmojis() 
            => (X.ToEmoji(), BallotBoxWithCheck.ToEmoji());
    }
}