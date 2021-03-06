using System.Collections.Generic;
using System.IO;

namespace unbis_discord_bot.Model
{
    class Links
    {
        public List<string> links { get; set; }
        public Links(ConfigJson config, ulong gId)
        {
            var fileName = config.quotePath + gId + " gif.txt";
            links = new List<string>();
            if (File.Exists(fileName))
            {
                foreach (var line in File.ReadLines(fileName))
                {
                    links.Add(line);
                }
            }
            else
            {
                File.Create(fileName).Dispose();
            }
        }
    }
}
