using System;
using Newtonsoft.Json;

namespace Volte.Data.Objects
{
    public sealed class Warn
    {
        public ulong User { get; set; }
        public string Reason { get; set; }
        public ulong Issuer { get; set; }
        public DateTimeOffset Date { get; set; }

        public override string ToString() 
            => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
