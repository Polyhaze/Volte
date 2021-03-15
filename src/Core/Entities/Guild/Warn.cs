using System;
using System.Text.Json;

namespace Volte.Core.Entities
{
    public sealed class Warn
    {
        public ulong User { get; init; }
        public string Reason { get; init; }
        public ulong Issuer { get; init; }
        public DateTimeOffset Date { get; init; }

        public override string ToString()
            => JsonSerializer.Serialize(this, Config.JsonOptions);
    }
}