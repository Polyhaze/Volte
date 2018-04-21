using Discord;
using System;
using System.Collections.Generic;

namespace SIVA.Core.Bot.Services.Database.DbTypes
{
    public class BotConfig : DatabaseEntity
    {
        public string Token { get; set; } = "";
        public string Prefix { get; set; } = "$";
        public bool Debug { get; set; } = true;
        public string BotGameToSet { get; set; } = "$h - @SIVA h";
        public string TwitchStreamer { get; set; } = "uGreem";
        public ulong BotOwner { get; set; } = 168548441939509248;
        public uint DefaultEmbedColour { get; set; } = 0x7000FB;
        public bool IsSelfbot { get; set; } = false;
        public string CurrencySymbol { get; set; } = "$";
        public ulong FeedbackChannelId { get; set; } = 0;
        public uint ErrorEmbedColour { get; set; } = 0xFF0000;
        public string LogSeverity { get; set; } = "verbose";
        public ulong[] Blacklist { get; set; } = {0, 0, 0, 0, 0, 0, 0, 0};
    }
}