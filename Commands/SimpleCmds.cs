using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class SimpleCmds : BaseCommandModule
    {
        [Command("banshee")]
        [Description("Sagt vorraus wer das Lieblingsziel der Banshee ist")]
        public async Task Banshee(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Die Banshee will die Ratte töten. Es ist eine gute Banshee! Hurra!!!").ConfigureAwait(false);
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
            await ctx.Channel.SendMessageAsync("Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack").ConfigureAwait(false);
        }

        [Command("mucke")]
        [Aliases("music","musik")]
        [Description(".mucke Proxy")]
        public async Task Mucke(CommandContext ctx, params string[] args)
        {
            string result = string.Empty;
            foreach (var text in args)
            {
                result = result + " " + text;
            }
            await ctx.Channel.SendMessageAsync(".mucke " + result).ConfigureAwait(false);
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
        [Description("Wer ist eigentlich dieser Toony?")]
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

        [Command("w20")]
        [Description("Wirft einen W20")]
        public async Task W20(CommandContext ctx)
        {
            var res = Shared.GenerateRandomNumber(1, 20);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + res).ConfigureAwait(false);
        }

        [Command("dice")]
        [Description("Wirft einen Würfel mit x Seiten")]
        public async Task Dice(CommandContext ctx, short num)
        {
            var res = Shared.GenerateRandomNumber(1, num);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + res).ConfigureAwait(false);
        }

        private static async Task PingPongInternal(CommandContext ctx, string target)
        {
            await ctx.Channel.SendMessageAsync("64 bytes from " + target + ": icmp_seq=1 ttl=120 time=" + Shared.GenerateRandomNumber(1, 10) + "." + Shared.GenerateRandomNumber(0, 99) + " ms").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("64 bytes from " + target + ": icmp_seq=2 ttl=120 time=" + Shared.GenerateRandomNumber(1, 10) + "." + Shared.GenerateRandomNumber(0, 99) + " ms").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("64 bytes from " + target + ": icmp_seq=3 ttl=120 time=" + Shared.GenerateRandomNumber(1, 10) + "." + Shared.GenerateRandomNumber(0, 99) + " ms").ConfigureAwait(false);
        }
    }
}
