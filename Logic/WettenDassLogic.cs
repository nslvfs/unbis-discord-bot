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

        public async Task AddUserToBet(ulong id, string vote, ulong amount, CommandContext ctx)
        {
            var configUser = GetUserFromDb(id);
            var user = CurWette.WettEinsaetze.FirstOrDefault(x => x.UserId == id);
            if (user == null)
            {
                user = new WettTeilnehmer();
                user.UserId = id;
                user.Vote = vote;
                user.Amount = amount;
            }
            if(amount > configUser.tokenBalance)
            {
                await ctx.Channel.SendMessageAsync("Du hast nicht genügend Tokens").ConfigureAwait(false);
                return;
            }
            configUser.tokenBalance -= amount;
            user.Amount += amount;

            if(vote == "yes")
                CurWette.yesPot += amount;

            if (vote == "no")
                CurWette.noPot += amount;

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
