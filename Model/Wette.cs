using System;
using System.Collections.Generic;

namespace unbis_discord_bot.Model
{
    public class Wette
    {
        public ulong UserIdStartedBet { get; set; }
        public string BetTopic { get; set; }
        public List<WettTeilnehmer> WettEinsaetze { get; set; }

        public bool wetteActive = false;

        public string curWettTopic = "";

        public DateTime BetStarted { get; set; }

        public ulong totalPot
        { get { return yesPot + noPot; } }

        public ulong yesPot { get; set; }
        public ulong noPot { get; set; }

        public double getOddsYes
        {
            get
            {
                double tempYes = (double)yesPot + 1.0;
                double tempNo = (double)noPot + 1.0;
                return Math.Round(tempNo / tempYes + 1.0, 2);
            }
        }

        public double getOddsNo
        {
            get
            {
                double tempYes = (double)yesPot + 1.0;
                double tempNo = (double)noPot + 1.0;
                return Math.Round(tempYes / tempNo + 1.0, 2);
            }
        }

        public ulong DealerCut
        {
            get
            {
                ulong temp = Convert.ToUInt64(totalPot * 0.1);
                return temp;
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
