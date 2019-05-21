using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Volte.Data.Models.Guild
{
    public sealed class GuildData
    {
        internal GuildData()
        {
            Configuration = new GuildConfiguration();
            Extras = new GuildExtras();
        }

        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("owner")]
        public ulong OwnerId { get; set; }

        [JsonProperty("configuration")]
        public GuildConfiguration Configuration { get; set; }

        [JsonProperty("extras")]
        public GuildExtras Extras { get; set; }

        public override string ToString()
            => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}