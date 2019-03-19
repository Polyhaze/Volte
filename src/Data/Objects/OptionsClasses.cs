using System.Collections.Generic;

namespace Volte.Data.Objects
{
    public sealed class VerificationOptions : object
    {
        public bool Enabled { get; set; }
        public ulong MessageId { get; set; }
        public ulong RoleId { get; set; }
    }

    public sealed class ModerationOptions : object
    {
        internal ModerationOptions()
        {
            Blacklist = new List<string>();
        }

        public bool MassPingChecks { get; set; }
        public bool Antilink { get; set; }
        public ulong ModRole { get; set; }
        public ulong AdminRole { get; set; }
        public List<string> Blacklist { get; set; }
    }

    public sealed class WelcomeOptions : object
    {
        public ulong WelcomeChannel { get; set; }
        public string WelcomeMessage { get; set; }
        public string LeavingMessage { get; set; }
        public int WelcomeColorR { get; set; }
        public int WelcomeColorG { get; set; }
        public int WelcomeColorB { get; set; }
    }
}