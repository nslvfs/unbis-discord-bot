using Newtonsoft.Json;

namespace unbis_discord_bot
{
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }
        [JsonProperty("rattenQuotePath")]
        public string rattenQuotePath { get; private set; }
    }
}

