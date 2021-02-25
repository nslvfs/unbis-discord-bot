using System.Collections.Generic;
using System.IO;

namespace unbis_discord_bot.Data
{
    class Quotes
    {
        public List<string> quotes { get; set; }

        public Quotes(ConfigJson config, ulong gId)
        {
            var fileName = config.quotePath + gId + ".txt";
            quotes = new List<string>();
            if (File.Exists(fileName))
            {
                foreach (var line in File.ReadLines(fileName))
                {
                    quotes.Add(line);
                }
            }
            else
            {
                File.Create(fileName).Dispose();
            }
        }
    }
}
