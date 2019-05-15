using System.Collections.Generic;
using Newtonsoft.Json;

namespace Volte.Data.Models
{
    public sealed class GuildConfiguration
    {
        internal GuildConfiguration()
        {
            ModerationOptions = new ModerationOptions();
            WelcomeOptions = new WelcomeOptions();
            SelfRoles = new List<string>();
            Tags = new List<Tag>();
            Warns = new List<Warn>();
        }

        public LiteDB.ObjectId Id { get; set; }
        public ulong ServerId { get; set; }
        public ulong GuildOwnerId { get; set; }
        public string Autorole { get; set; }
        public string CommandPrefix { get; set; }
        public WelcomeOptions WelcomeOptions { get; set; }
        public ModerationOptions ModerationOptions { get; set; }
        public bool DeleteMessageOnCommand { get; set; }
        public List<string> SelfRoles { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Warn> Warns { get; set; }

        public override string ToString() 
            => JsonConvert.SerializeObject(this, Formatting.Indented);

    }
}