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

        [JsonProperty("xSichterPath")]
        public string xSichterPath { get; private set; }

        [JsonProperty("gurgleApi")]
        public string gurgleApi { get; private set; }

        [JsonProperty("gurgleSeachEngineId")]
        public string gurgleSeachEngineId { get; private set; }

        [JsonProperty("badwords")]
        public string badwords { get; private set; }

        [JsonProperty("negativAnswer")]
        public string negativAnswer { get; set; }

        [JsonProperty("positivAnswer")]
        public string positivAnswer { get; set; }

        [JsonProperty("wettenDBFile")]
        public string wettenDBFile { get; set; }

    }
}

