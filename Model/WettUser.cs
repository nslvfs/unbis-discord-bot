using Newtonsoft.Json;
using System;

namespace unbis_discord_bot.Model
{
    public class WettUser
    {
        [JsonProperty("id")]
        public long id { get; set; }
        [JsonProperty("tokenBalance")]
        public ulong tokenBalance { get; set; }
        [JsonProperty("lastReceived")]
        public DateTime lastReceived { get; set; }
    }
}
