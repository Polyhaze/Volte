using System.Collections.Generic;
using LiteDB;
using Newtonsoft.Json;

namespace Volte.Data.Objects
{
    public sealed class DiscordServer
    {
        internal DiscordServer()
        {
            ModerationOptions = new ModerationOptions();
            WelcomeOptions = new WelcomeOptions();
            SelfRoles = new List<string>();
            Tags = new List<Tag>();
        }

        public ObjectId Id { get; set; }
        public ulong ServerId { get; set; }
        public ulong GuildOwnerId { get; set; }
        public string Autorole { get; set; }
        public string CommandPrefix { get; set; }
        public WelcomeOptions WelcomeOptions { get; set; }
        public ModerationOptions ModerationOptions { get; set; }
        public VerificationOptions VerificationOptions { get; set; }
        public bool DeleteMessageOnCommand { get; set; }
        public List<string> SelfRoles { get; set; }
        public List<Tag> Tags { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}