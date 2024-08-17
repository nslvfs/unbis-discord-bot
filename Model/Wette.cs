using System;
using System.Collections.Generic;

namespace unbis_discord_bot.Model
{
    public class Wette
    {
        public long UserIdStartedBet { get; set; }
        public string BetTopic { get; set; }
        public List<WettTeilnehmer> WettEinsaetze { get; set; }

        public bool wetteActive = false;

        public string curWettTopic = "";

        public long totalPot
        { get { return yesPot + noPot; } }

        public long yesPot { get; set; }
        public long noPot { get; set; }

        public double getOddsYes
        {
            get
            {
                double tempYes = (double)yesPot + 100.0;
                double tempNo = (double)noPot + 100.0;
                return Math.Round(tempNo / tempYes + 1.0, 2);
            }
        }

        public double getOddsNo
        {
            get
            {
                double tempYes = (double)yesPot + 100.0;
                double tempNo = (double)noPot + 100.0;
                return Math.Round(tempYes / tempNo + 1.0, 2);
            }
        }

        public Wette()
        {
            WettEinsaetze = new List<WettTeilnehmer>();
            yesPot = 0;
            noPot = 0;
        }

    }

    public class WettTeilnehmer
    {
        public long UserId { get; set; }
        public ulong WettEinsatz { get; set; }
        public string vote { get; set; }
    }
}
