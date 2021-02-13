using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class SimpleCmds : BaseCommandModule
    {
        private static List<Model.PerChannelRoulette> rouletteData = new List<Model.PerChannelRoulette>();

        [Command("banshee")]
        [Description("Sagt vorraus wer das Lieblingsziel der Banshee ist")]
        public async Task Banshee(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Die Banshee will die Ratte töten. Es ist eine gute Banshee! Hurra!!!").ConfigureAwait(false);
        }

        [Command("rauchen")]
        [Description("Jetzt ein Kippchen?")]
        public async Task Rauchen(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Ja").ConfigureAwait(false);
        }

        [Command("arbeiten")]
        [Description("Arbeiten?")]
        public async Task Arbeiten(CommandContext ctx)
        {
            if (ctx.Member.IsOwner) {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Nein").ConfigureAwait(false);
            } else {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Ja").ConfigureAwait(false);
            }
        }

        [Command("nappen")]
        [Description("Nappen?")]
        public async Task Nappen(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Ja").ConfigureAwait(false);
        }

        [Command("feierabend")]
        [Description("Feierabend?")]
        public async Task Feierabend(CommandContext ctx)
        {
            if (ctx.Member.IsOwner)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Ja").ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Nein").ConfigureAwait(false);
            }
        }

        [Command("werwach")]
        [Description("Wer ist noch Wach?")]
        public async Task WerWach(CommandContext ctx)
        {
            var roles = ctx.Guild.Roles;
            foreach (var role in roles)
            {
                if (role.Value.Name == "@everyone")
                {
                    await ctx.Channel.SendMessageAsync("Wer ist wach @everyone?").ConfigureAwait(false);
                }
            }

        }

        [Command("fork")]
        [Aliases("github")]
        [Description("Jetzt ein Kippchen?")]
        public async Task Fork(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": https://github.com/nslvfs/unbis-discord-bot").ConfigureAwait(false);
        }

        [Command("backlog")]
        [Description("Keine Beschreibung")]
        public async Task Backlog(CommandContext ctx)
        {
            var res = Shared.GenerateRandomNumber(1, 100);
            if (res <= 50)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Backlog ist Krebs").ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Scroll halt hoch du Pansen").ConfigureAwait(false);
            }
        }

        [Command("spiegel")]
        [Description("NEIN DU!")]
        public async Task Spiegel(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("NEIN DU!").ConfigureAwait(false);
        }

        [Command("roulette")]
        [Aliases("r")]
        [Description("Russischer Familienspaß")]
        public async Task Roulette(CommandContext ctx)
        {
            var found = false;
            foreach(var item in rouletteData)
            {
              if(item.channelId == ctx.Channel.Id)
                {
                    found = true;
                    if (item.revKammer == -1)
                    {
                        var text = DSharpPlus.Formatter.Italic("lädt den Revolver");
                        await ctx.Channel.SendMessageAsync(text).ConfigureAwait(false);
                        item.revKammer = Shared.GenerateRandomNumber(1, 8);
                        item.revShots = 0;
                    }
                    item.revShots++;
                    if (item.revShots != item.revKammer)
                    {
                        await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": ** klick **").ConfigureAwait(false);
                    }
                    if (item.revShots == item.revKammer)
                    {
                        item.revKammer = -1;
                        await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": ** BOOM **").ConfigureAwait(false);
                    }
                    break;
                }
            }
            if(!found)
            {
                Model.PerChannelRoulette newItem = new Model.PerChannelRoulette();
                newItem.channelId = ctx.Channel.Id;
                rouletteData.Add(newItem);
                await Roulette(ctx);
            }

        }

        [Command("nachti")]
        [Aliases("guna", "n8")]
        [Description("Gute Nacht")]
        public async Task Nachti(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": guna schlagu").ConfigureAwait(false);
        }

        [Command("ping")]
        [Aliases("pong")]
        [Description("Ping!")]
        public async Task PingPong(CommandContext ctx, DiscordUser target)
        {
            await PingPongInternal(ctx, target.Mention);
        }

        [Command("ping")]
        [Description("Ping!")]
        public async Task PingPong(CommandContext ctx, string target)
        {
            await PingPongInternal(ctx, target);
        }

        [Command("ping")]
        [Description("Ping!")]
        public async Task PingPong(CommandContext ctx)
        {
            await PingPongInternal(ctx, ctx.Member.Mention);
        }

        [Command("haltdiefresseeinkleinerratten")]
        [Description("Jetzt ein Kippchen?")]
        public async Task HdfRatten(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Halt die Fresse kleiner Ratten").ConfigureAwait(false);
        }

        [Command("sack")]
        [Description("Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack ")]
        public async Task MeinSack(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack ").ConfigureAwait(false);
        }

        [Command("hdf")]
        [Description("Jemand höfflich darum bitte ruhig zu sein")]
        public async Task Hdf(CommandContext ctx, DiscordUser target)
        {
            await ctx.Channel.SendMessageAsync("Halt die Fresse " + target.Mention).ConfigureAwait(false);
        }

        [Command("hdf")]
        [Description("Jemand höfflich darum bitte ruhig zu sein")]
        public async Task Hdf(CommandContext ctx, string target)
        {
            await ctx.Channel.SendMessageAsync("Halt die Fresse " + target).ConfigureAwait(false);
        }

        [Command("Piep")]
        [Description("Wo sind die susmäuse")]
        public async Task Piep(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Alle Susmäuse machen jetzt piep :mouse: ").ConfigureAwait(false);
            var text = DSharpPlus.Formatter.Italic("piept");
            await ctx.Channel.SendMessageAsync(text).ConfigureAwait(false);
        }

        [Command("jn")]
        [Description("Beantwortet eine Ja/Nein Frage")]
        public async Task JaNein(CommandContext ctx)
        {
            var res = Shared.GenerateRandomNumber(1, 100);
            if(res <= 50)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Nein").ConfigureAwait(false);
            } else if ( res <= 98)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Ja").ConfigureAwait(false);
            } else
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Keine Ahnung, halts Maul").ConfigureAwait(false);
            }
        }

        [Command("Stoll")]
        [Description("Gibt ein Zitat von Dr. Axel Stoll wieder.")]
        public async Task QuoteStoll(CommandContext ctx)
        {
            var temp = new Data.Stoll();
            var res = Shared.GenerateRandomNumber(0, temp.array.Length - 1);
            await ctx.Channel.SendMessageAsync("Dr. Axel Stoll, promovierter Naturwissenschaftler, sagt: " + temp.array[res]).ConfigureAwait(false);
        }

        [Command("Seeliger")]
        [Description("Gibt ein Zitat von Julia Seeliger wieder.")]
        public async Task QuoteSeeliger(CommandContext ctx)
        {
            var temp = new Data.Seeliger();
            var res = Shared.GenerateRandomNumber(0, temp.array.Length - 1);
            await ctx.Channel.SendMessageAsync("Julia Seeliger sagt: " + temp.array[res]).ConfigureAwait(false);
        }

        [Command("Quote")]
        [Aliases("Kevin")]
        [Description("Gibt ein zufälliges Zitat wieder.")]
        public async Task QuoteRattendiscord(CommandContext ctx)
        {
            var temp = new Data.Quotes(Bot.configJson, ctx.Guild.Id);
            if(temp.quotes.Count > 0)
            {
                var res = Shared.GenerateRandomNumber(0, temp.quotes.Count - 1);
                if(ctx.Guild.Id == 442300530996543489) { 
                    await ctx.Channel.SendMessageAsync("Kevin sagt: " + temp.quotes[res]).ConfigureAwait(false);
                } else
                {
                    await ctx.Channel.SendMessageAsync("Konfuzius sagt: " + temp.quotes[res]).ConfigureAwait(false);
                }
            } else
            {
                await ctx.Channel.SendMessageAsync("Keine Quotes gefunden :c");
            }

        }

        [Command("Addquote")]
        [Description("Fügt ein Zitat zu !quote hinzu.")]
        public async Task QuoteAdd(CommandContext ctx, params string[] args)
        {
            string result = string.Empty;
            var fileName = Bot.configJson.quotePath + ctx.Guild.Id + ".txt";
            foreach (var arg in args)
            {
                result = result + " " + arg;
            }
            
            if (!File.Exists(fileName)) {
                File.Create(fileName).Dispose();
            } 
            
            using (StreamWriter w = File.AppendText(fileName))
            {
                w.WriteLine(result.Trim());
            }
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Quote hinzugefügt du Pisser!");
        }

        [Command("getRandomNumber()")]
        [Description("Generiert eine zufällige Zahl")]
        public async Task GetRandomNumber(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": 9").ConfigureAwait(false);
        }

        [Command("f")]
        [Description("Pays Respect")]
        public async Task PayRespect(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(":pray:").ConfigureAwait(false);
        }

        [Command("toony")]
        [Description("Wer ist eigentlich dieser  Toony?")]
        public async Task Toony(CommandContext ctx)
        {
            var res = Shared.GenerateRandomNumber(1, 100);
            if (res > 10)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Toony ist ein Hurensohn").ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Du alte Pizza Funghi").ConfigureAwait(false);
            }
        }

        private static async Task PingPongInternal(CommandContext ctx, string target)
        {
            await ctx.Channel.SendMessageAsync("64 bytes from " + target + ": icmp_seq=1 ttl=120 time=" + Shared.GenerateRandomNumber(1, 10) + "." + Shared.GenerateRandomNumber(0, 99) + " ms").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("64 bytes from " + target + ": icmp_seq=2 ttl=120 time=" + Shared.GenerateRandomNumber(1, 10) + "." + Shared.GenerateRandomNumber(0, 99) + " ms").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("64 bytes from " + target + ": icmp_seq=3 ttl=120 time=" + Shared.GenerateRandomNumber(1, 10) + "." + Shared.GenerateRandomNumber(0, 99) + " ms").ConfigureAwait(false);
        }
    }
}
