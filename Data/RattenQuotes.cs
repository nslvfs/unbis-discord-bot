using System.Collections.Generic;
using System.IO;

namespace unbis_discord_bot.Data
{
    class RattenQuotes
    {
        public List<string> quotes { get; set; }

        public RattenQuotes(ConfigJson config)
        {
            quotes = new List<string>();           
            if (File.Exists(config.rattenQuotePath)) {
                foreach (var line in File.ReadLines(config.rattenQuotePath))
                {
                    quotes.Add(line);
                }
            } 
        }
    }
}
