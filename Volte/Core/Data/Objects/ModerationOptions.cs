using System;
using System.Collections.Generic;
using System.Text;

namespace Volte.Core.Data.Objects
{
    public sealed class ModerationOptions
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
}
