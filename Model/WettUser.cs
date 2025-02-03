using Newtonsoft.Json;
using System;

namespace unbis_discord_bot.Model
{
    public class WettUser
    {
        [JsonProperty(nameof(id))]
        public ulong id { get; set; }
        [JsonProperty(nameof(tokenBalance))]
        public ulong tokenBalance { get; set; }
        [JsonProperty(nameof(lastReceived))]
        public DateTime lastReceived { get; set; }
    }
}
