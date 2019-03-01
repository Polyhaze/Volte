using System;
using System.Collections.Generic;
using System.Text;

namespace Volte.Core.Data.Objects
{
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
