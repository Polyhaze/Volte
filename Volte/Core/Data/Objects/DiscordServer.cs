using System.Collections.Generic;
using LiteDB;
using Newtonsoft.Json;

namespace Volte.Core.Data.Objects
{
    public class DiscordServer : object
    {
        #region ValueDeclaration

        public DiscordServer()
        {
            SelfRoles = new List<string>();
            Blacklist = new List<string>();
            Tags = new List<Tag>();
        }

        public ObjectId Id { get; set; }
        public ulong ServerId { get; set; }
        public ulong GuildOwnerId { get; set; }
        public string Autorole { get; set; }
        public string CommandPrefix { get; set; }
        public bool Leveling { get; set; }
        public ulong WelcomeChannel { get; set; }
        public string WelcomeMessage { get; set; }
        public string LeavingMessage { get; set; }
        public int WelcomeColorR { get; set; }
        public int WelcomeColorG { get; set; }
        public int WelcomeColorB { get; set; }
        public bool MassPingChecks { get; set; }
        public bool Antilink { get; set; }
        public ulong ModRole { get; set; }
        public ulong AdminRole { get; set; }
        public bool DeleteMessageOnCommand { get; set; }
        public List<string> SelfRoles { get; set; }
        public List<string> Blacklist { get; set; }
        public List<Tag> Tags { get; set; }

        #endregion

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}