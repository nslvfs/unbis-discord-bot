using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace unbis_discord_bot.Model
{
    public class Message
    {
        public DiscordUser Author { get; set; }
        public ulong ChannelId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
