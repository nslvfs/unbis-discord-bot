using System.Text.Json.Serialization;

namespace unbis_discord_bot
{
    public class ConfigJson
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("prefix")]
        public string Prefix { get; set; }

        [JsonPropertyName("quotePath")]
        public string quotePath { get; set; }

        [JsonPropertyName("xSichterPath")]
        public string xSichterPath { get; set; }

        [JsonPropertyName("gurgleApi")]
        public string gurgleApi { get; set; }

        [JsonPropertyName("gurgleSeachEngineId")]
        public string gurgleSeachEngineId { get; set; }

        [JsonPropertyName("badwords")]
        public string badwords { get; set; }

        [JsonPropertyName("negativAnswer")]
        public string negativAnswer { get; set; }

        [JsonPropertyName("positivAnswer")]
        public string positivAnswer { get; set; }

        [JsonPropertyName("wettenDBFile")]
        public string wettenDBFile { get; set; }

        [JsonPropertyName("guildIdUnbi")]
        public ulong GuildIdUnbi { get; set; } = 791393115097137171;

        [JsonPropertyName("guildIdRatte")]
        public ulong GuildIdRatte { get; set; } = 442300530996543489;

        [JsonPropertyName("channelIdRotz")]
        public ulong ChannelIdRotz { get; set; } = 816990123568660510;

        [JsonPropertyName("userIdvfs")]
        public ulong UserIdVfs { get; set; } = 134719067016658945;

        [JsonPropertyName("botIdSelf")]
        public ulong BotIdSelf { get; set; } = 807641560006000670;

        [JsonPropertyName("botIdSantos")]
        public ulong BotIdSantos { get; set; } = 816383436201656341;

        [JsonPropertyName("roleIdMuted")]
        public ulong RoleIdMuted { get; set; } = 807921762570469386;
    }
}

