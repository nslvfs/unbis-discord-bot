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

        public ulong TotalPot
        { get { return YesPot + NoPot; } }

        public ulong YesPot { get; set; }
        public ulong NoPot { get; set; }

        public double GetOddsYes
        {
            get
            {
                double tempYes = (double)YesPot + 1.0;
                double tempNo = (double)NoPot + 1.0;
                return Math.Round(tempNo / tempYes + 1.0, 2);
            }
        }

        public double GetOddsNo
        {
            get
            {
                double tempYes = (double)YesPot + 1.0;
                double tempNo = (double)NoPot + 1.0;
                return Math.Round(tempYes / tempNo + 1.0, 2);
            }
        }

        public ulong DealerCut
        {
            get
            {
                ulong temp = Convert.ToUInt64(TotalPot * 0.1);
                return temp;
            }
        }

        public Wette()
        {
            WettEinsaetze = [];
            YesPot = 0;
            NoPot = 0;
        }
    }

    public class WettTeilnehmer
    {
        public ulong UserId { get; set; }
        public ulong Amount { get; set; }
        public string Vote { get; set; }
    }
}
