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
        private ConfigJson Config { get; set; }
        public List<WettUser> DbData { get; set; }

        public Wette CurWette { get; set; }

        public WettenDassLogic()
        {
            ReadFile();
            CurWette = new Wette();
        }
        private void ReadFile()
        {
            ReadConfig();
            var file = Config.wettenDBFile;
            var json = String.Empty;
            if (File.Exists(file))
            {
                using (var fs = File.OpenRead(file))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();
                DbData = JsonConvert.DeserializeObject<List<WettUser>>(json);
                return;
            }
            DbData = new List<WettUser>();
        }

        private void WriteFile()
        {
            ReadConfig();
            var file = Config.wettenDBFile;
            var json = JsonConvert.SerializeObject(DbData.ToArray());
            File.WriteAllText(file, json);
        }

        private void ReadConfig()
        {
            if (Config.wettenDBFile != null)
            {
                var json = string.Empty;
                using (var fs = File.OpenRead("config.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();

                Config = JsonConvert.DeserializeObject<ConfigJson>(json);
            }
        }
    }
}
