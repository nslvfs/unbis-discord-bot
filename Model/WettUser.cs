using System;
using System.Text.Json.Serialization;

namespace unbis_discord_bot.Model
{
    public class WettUser
    {
        [JsonPropertyName(nameof(id))]
        public ulong id { get; set; }
        [JsonPropertyName(nameof(tokenBalance))]
        public ulong tokenBalance { get; set; }
        [JsonPropertyName(nameof(lastReceived))]
        public DateTime lastReceived { get; set; }
    }
}
