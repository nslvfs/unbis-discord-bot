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
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Backlog ist Krebs").ConfigureAwait(false);
        }

        [Command("belastung")]
        [Description("Wie belastend ist es gerade")]
        public async Task Belastung(CommandContext ctx)
        {

            if (ctx.Member.Id == Bot.userIdvfs)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Es ist alles sehr belastend").ConfigureAwait(false);
                return;
            }
            var res = Shared.GenerateRandomNumber(80, 100);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + res + "%").ConfigureAwait(false);
        }

        [Command("nachti")]
        [Aliases("guna", "n8", "gn8")]
        [Description("Gute Nacht")]
        public async Task Nachti(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": guna schlagu").ConfigureAwait(false);
        }

        [Command("gumo")]
        [Description("Guten Morgen")]
        public async Task Gumo(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Guten Morgen ❤ Erstma 🚬").ConfigureAwait(false);
        }

        [Command("sack")]
        [Description("Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack ")]
        public async Task MeinSack(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack Mein Sack").ConfigureAwait(false);
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

        [Command("encode")]
        [Aliases("enc")]
        [Description("verschlüßel einen text")]
        public async Task EncodeMesage(CommandContext ctx, [RemainingText] string qry)
        {
            if (Bot.CheckBadWords(qry))
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + Bot.configJson.negativAnswer).ConfigureAwait(false);
                return;
            }
            string encryptedstring = Logic.Encryption.Encrypt(qry, Bot.configJson.cryptoPwd);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + " " + encryptedstring).ConfigureAwait(false);
        }

        [Command("decode")]
        [Aliases("dec")]
        [RequireOwner]
        [Description("verschlüßel einen text")]
        public async Task DecodeMesage(CommandContext ctx, [RemainingText] string qry)
        {
            string decryptedstring = Logic.Encryption.Decrypt(qry, Bot.configJson.cryptoPwd);
            if (Bot.CheckBadWords(decryptedstring))
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + Bot.configJson.negativAnswer).ConfigureAwait(false);
                return;
            }
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + " " + decryptedstring).ConfigureAwait(false);
        }
    }
}
