namespace unbis_discord_bot.Model
{
    public class PerChannelRoulette
    {
        public ulong  channelId { get; set; }
        public int revShots { get; set; }
        public int revKammer { get; set; }

        public PerChannelRoulette()
        {
            revKammer = -1;
            revShots = 0;
        }
    }
}
