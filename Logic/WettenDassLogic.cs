using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using unbis_discord_bot.Model;

namespace unbis_discord_bot.Logic
{
    public class WettenDassLogic
    {
        private ConfigJson configJson { get; set; }
        public List<WettUser> dbData { get; set; }

        public WettenDassLogic()
        {
            ReadFile();
        }
        private void ReadFile()
        {
            ReadConfig();
            var file = configJson.wettenDBFile;
            var json = String.Empty;
            if (File.Exists(file))
            {
                using (var fs = File.OpenRead(file))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();
                dbData = JsonConvert.DeserializeObject<List<WettUser>>(json);
                return;
            }
            dbData = new List<WettUser>();
        }

        private void WriteFile()
        {
            ReadConfig();
            var file = configJson.wettenDBFile;
            var json = JsonConvert.SerializeObject(dbData.ToArray());
            File.WriteAllText(file, json);
        }

        private void ReadConfig()
        {
            if (configJson.wettenDBFile != null)
            {
                var json = string.Empty;
                using (var fs = File.OpenRead("config.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();

                configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
            }
        }
    }
}
