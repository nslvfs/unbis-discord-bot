using Newtonsoft.Json;

namespace unbis_discord_bot
{
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        public string Prefix { get; private set; }

        [JsonProperty("quotePath")]
        public string quotePath { get; private set; }

        [JsonProperty("blogContent")]
        public string blogContent { get; private set; }

        [JsonProperty("blogRss")]
        public string blogRss { get; private set; }

        [JsonProperty("cryptoPwd")]
        public string cryptoPwd { get; private set; }
    }
}

