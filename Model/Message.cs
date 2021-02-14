using DSharpPlus.Entities;
using System;

namespace unbis_discord_bot.Model
{
    public class Message
    {
        public DiscordUser Author { get; set; }
        public ulong ChannelId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
