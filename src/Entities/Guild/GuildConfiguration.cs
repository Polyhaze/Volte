﻿using System.Text.Json.Serialization;
using Gommon;

namespace Volte.Entities
{
    public sealed class GuildConfiguration
    {
        internal GuildConfiguration()
        {
            Moderation = new ModerationOptions();
            Welcome = new WelcomeOptions();
            Starboard = new StarboardOptions();
        }

        [JsonPropertyName("autorole")]
        public ulong Autorole { get; set; }

        [JsonPropertyName("command_prefix")]
        public string CommandPrefix { get; set; }

        [JsonPropertyName("welcome_options")]
        public WelcomeOptions Welcome { get; set; }

        [JsonPropertyName("moderation_options")]
        public ModerationOptions Moderation { get; set; }

        [JsonPropertyName("starboard_options")]
        public StarboardOptions Starboard { get; set; }

        [JsonPropertyName("reply_inline")]
        public bool ReplyInline { get; set; }

        [JsonPropertyName("embed_tags_and_show_its_author")]
        public bool EmbedTagsAndShowAuthor { get; set; }
        
        public override string ToString() => this.AsJson();

    }
}