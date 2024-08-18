using DSharpPlus.CommandsNext;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<bool> AddUserToBet(ulong id, string vote, ulong amount, CommandContext ctx)
        {
            var configUser = GetUserFromDb(id);
            var wettUser = CurWette.WettEinsaetze.FirstOrDefault(x => x.UserId == id);

            if (wettUser == null)
            {
                wettUser = new WettTeilnehmer();
                wettUser.UserId = id;
                wettUser.Vote = vote;
                wettUser.Amount = 0;
                CurWette.WettEinsaetze.Add(wettUser);
            }

            if (vote != wettUser.Vote)
            {
                await ctx.Channel.SendMessageAsync("Du kannst nicht auf beide Ergebnisse Wetten").ConfigureAwait(false);
                return false;
            }

            if (amount > configUser.tokenBalance)
            {
                await ctx.Channel.SendMessageAsync("Du hast nicht genügend Token.").ConfigureAwait(false);
                return false;
            }

            configUser.tokenBalance -= amount;
            wettUser.Amount += amount;

            if (vote == "ja")
                CurWette.yesPot += amount;

            if (vote == "nein")
                CurWette.noPot += amount;

            WriteFile();
            return true;
        }

        public void TokenGiveOut()
        {
            foreach (var user in DbData)
            {
                var diff = (DateTime.Now - user.lastReceived).TotalDays;
                if (diff >= 7)
                {
                    user.tokenBalance += 1000;
                    user.lastReceived = DateTime.Now;
                }
            }
            WriteFile();
        }

        public WettUser GetUserFromDb(ulong id)
        {
            var configUser = DbData.FirstOrDefault(x => x.id == id);
            if (configUser == null)
            {
                configUser = new WettUser();
                configUser.id = id;
                configUser.lastReceived = DateTime.Now;
                configUser.tokenBalance = 1000;
                DbData.Add(configUser);
                WriteFile();
            }
            return configUser;
        }

        public void ReadFile()
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

        public void WriteFile()
        {
            ReadConfig();
            var file = Config.wettenDBFile;
            var json = JsonConvert.SerializeObject(DbData.ToArray());
            File.WriteAllText(file, json);
        }
        private void ReadConfig()
        {
            if (Config.wettenDBFile == null)
            {
                var json = string.Empty;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
                using (var fs = File.OpenRead(path))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();

                Config = JsonConvert.DeserializeObject<ConfigJson>(json);
            }
        }
    }
}
