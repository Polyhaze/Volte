using System;
using System.Text.Json;

namespace Volte.Core.Entities
{
    public sealed class Warn
    {
        public ulong User { get;  set; }
        public string Reason { get; set; }
        public ulong Issuer { get; set; }
        public DateTimeOffset Date { get; set; }

        public override string ToString()
            => JsonSerializer.Serialize(this, Config.JsonOptions);
    }
}