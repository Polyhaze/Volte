using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Core.UserAccounts
{
    public class UserAccount
    {
        public UserAccount()
        {
            Warns = new List<string>();
        }

        public ulong ID { get; set; }

        public uint Points { get; set; }

        public uint XP { get; set; }

        public uint LevelNumber
        {
            get
            {
                return (uint)Math.Sqrt(XP / 50);
            }
        }

        public List<string> Warns { get; set; }

        public uint WarnCount { get; set; }

        public int Money { get; set; }
    }
}
