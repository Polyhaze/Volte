using System;
using Discord;
 
using Newtonsoft.Json;

namespace Volte.Core.Models.Guild
{
    public sealed class Warn
    {
        public ulong User { get; set; }
        public string Reason { get; set; }
        public ulong Issuer { get; set; }
        public DateTimeOffset Date { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}