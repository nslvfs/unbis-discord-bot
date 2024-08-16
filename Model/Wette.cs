using System.Collections.Generic;

namespace unbis_discord_bot.Model
{
    public class Wette
    {
        public long UserIdStartedBet { get; set; }
        public string BetTopic { get; set; }
        public List<WettTeilnehmer> WettEinsaetze { get; set; }

        public Wette()
        {
            WettEinsaetze = new List<WettTeilnehmer>();
        }

    }

    public class WettTeilnehmer
    {
        public long UserId { get; set; }
        public ulong WettEinsatz { get; set; }
    }
}
