using System.Collections.Generic;
using Newtonsoft.Json;

namespace Volte.Data.Models.Guild
{
    internal sealed class DummyGuildConfig
    {
        internal DummyGuildConfig()
        {
            ModerationOptions = new DummyModerationOptions();
            WelcomeOptions = new DummyWelcomeOptions();
            SelfRoles = new List<string>();
            Tags = new List<Tag>();
            Warns = new List<Warn>();
        }

        public LiteDB.ObjectId Id { get; set; }
        public ulong ServerId { get; set; }
        public ulong GuildOwnerId { get; set; }
        public string Autorole { get; set; }
        public string CommandPrefix { get; set; }
        public DummyWelcomeOptions WelcomeOptions { get; set; }
        public DummyModerationOptions ModerationOptions { get; set; }
        public bool DeleteMessageOnCommand { get; set; }
        public List<string> SelfRoles { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Warn> Warns { get; set; }

        public override string ToString()
            => JsonConvert.SerializeObject(this, Formatting.Indented);

    }

    internal sealed class DummyModerationOptions
    {
        internal DummyModerationOptions()
        {
            Blacklist = new List<string>();
        }

        public bool MassPingChecks { get; set; }
        public bool Antilink { get; set; }
        public ulong ModActionLogChannel { get; set; }
        public ulong ModActionCaseNumber { get; set; }
        public ulong ModRole { get; set; }
        public ulong AdminRole { get; set; }
        public List<string> Blacklist { get; set; }
    }

    internal sealed class DummyWelcomeOptions
    {
        public ulong WelcomeChannel { get; set; }
        public string WelcomeMessage { get; set; }
        public string LeavingMessage { get; set; }
        public int WelcomeColorR { get; set; }
        public int WelcomeColorG { get; set; }
        public int WelcomeColorB { get; set; }
    }
}
