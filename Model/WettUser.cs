using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace unbis_discord_bot.Model
{
    public class WettUser
    {
        [JsonProperty("id")]
        public long id { get; set; }
        [JsonProperty("token")]
        public ulong token {  get; set; }
        [JsonProperty("lastReceived")]
        public DateTime lastReceived { get; set; }
    }
}
