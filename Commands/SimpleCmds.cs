using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class SimpleCmds : BaseCommandModule
    {
        private static int revShots = 0;
        private static int revKammer = -1;

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
        [Description("Russischer Familienspaß")]
        public async Task Roulette(CommandContext ctx)
        {
            if(revKammer == -1)
            {
                var text = DSharpPlus.Formatter.Italic("lädt den Revolver");
                await ctx.Channel.SendMessageAsync(text).ConfigureAwait(false);
                revKammer = Shared.GenerateRandomNumber(1, 8);
                revShots = 0;
            }
            revShots++;
            if(revShots != revKammer)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention +  ": ** klick **").ConfigureAwait(false);
            }
            if (revShots == revKammer)
            {
                revKammer = -1;
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": ** BOOM **").ConfigureAwait(false);
            }


        }

        [Command("nachti")]
        [Description("Gute Nacht")]
        public async Task Nachti(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": guna schlagu").ConfigureAwait(false);
        }


        [Command("haltdiefresseeinkleinerratten")]
        [Description("Jetzt ein Kippchen?")]
        public async Task HdfRatten(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Halt die Fresse kleiner Ratten").ConfigureAwait(false);
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

        [Command("jn")]
        [Description("Beantwortet eine Ja/Nein Frage")]
        public async Task JaNein(CommandContext ctx)
        {
            var res = Shared.GenerateRandomNumber(1, 100);
            if(res <= 50)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Nein").ConfigureAwait(false);
            } else
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Ja").ConfigureAwait(false);
            }
        }

        [Command("Kevin")]
        [Description("Gibt ein originales Kevin-Zitat wieder.")]
        public async Task QuoteKevin(CommandContext ctx)
        {
            var temp = new Data.Stoll();
            var res = Shared.GenerateRandomNumber(0, temp.array.Length - 1);
            await ctx.Channel.SendMessageAsync("Kevin sagt: " + temp.array[res]).ConfigureAwait(false);

        }

        [Command("Stoll")]
        [Description("Gibt ein Zitat von Dr. Axel Stoll wieder.")]
        public async Task QuoteStoll(CommandContext ctx)
        {
            var temp = new Data.Stoll();
            var res = Shared.GenerateRandomNumber(0, temp.array.Length - 5);
            await ctx.Channel.SendMessageAsync("Dr. Axel Stoll, promovierter Naturwissenschaftler, sagt: " + temp.array[res]).ConfigureAwait(false);

        }

        [Command("Seeliger")]
        [Description("Gibt ein Zitat von Julia Seeliger wieder.")]
        public async Task QuoteSeeliger(CommandContext ctx)
        {
            var temp = new Data.Seeliger();
            var res = Shared.GenerateRandomNumber(0, temp.array.Length - 5);
            await ctx.Channel.SendMessageAsync("Julia Seeliger sagt: " + temp.array[res]).ConfigureAwait(false);

        }

        [Command("Quote")]
        [Description("Gibt ein zufälliges Zitat wieder.")]
        public async Task QuoteRattendiscord(CommandContext ctx)
        {
            var temp = new Data.RattenQuotes();
            var res = Shared.GenerateRandomNumber(0, temp.array.Length - 1);
            await ctx.Channel.SendMessageAsync("Kevin sagt: " + temp.array[res]).ConfigureAwait(false);

        }

        [Command("getRandomNumber()")]
        [Description("Generiert eine zufällige Zahl")]
        public async Task GetRandomNumber(CommandContext ctx)
        {

            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": 9").ConfigureAwait(false);
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
    }
}
