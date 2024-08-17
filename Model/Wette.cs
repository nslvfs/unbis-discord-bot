using System;
using System.Collections.Generic;
using System.Linq;

namespace unbis_discord_bot.Model
{
    public class Wette
    {
        public long UserIdStartedBet { get; set; }
        public string BetTopic { get; set; }
        public List<WettTeilnehmer> WettEinsaetze { get; set; }

        public bool wetteActive = false;

        public string curWettTopic = "";

        public ulong totalPot
        { get { return yesPot + noPot; } }

        public ulong yesPot { get; set; }
        public ulong noPot { get; set; }

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
        public ulong UserId { get; set; }
        public ulong Amount { get; set; }
        public string Vote { get; set; }
    }
}
