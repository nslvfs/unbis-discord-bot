using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.IO;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class Quotes : BaseCommandModule
    {
        [Command("Quote")]
        [Aliases("Kevin")]
        [Description("Gibt ein zufälliges Zitat wieder.")]
        public async Task Quote(CommandContext ctx)
        {
            var temp = new Data.Quotes(Bot.configJson, ctx.Guild.Id);
            if (temp.quotes.Count > 0)
            {
                var res = Shared.GenerateRandomNumber(0, temp.quotes.Count - 1);
                if (ctx.Guild.Id == 442300530996543489)
                {
                    await ctx.Channel.SendMessageAsync("Kevin sagt: " + temp.quotes[res]).ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendMessageAsync("Konfuzius sagt: " + temp.quotes[res]).ConfigureAwait(false);
                }
            }
            else
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
            if (result.Length <= 3)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Das ist kein Quote");
                return;
            }
            result = result + " (von " + ctx.Member.Mention + " hinzugefügt)";

            if (!File.Exists(fileName))
            {
                File.Create(fileName).Dispose();
            }

            using (StreamWriter w = File.AppendText(fileName))
            {
                w.WriteLine(result.Trim());
            }
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Quote hinzugefügt du Pisser!");
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

        [Command("Bash")]
        [Description("Gibt ein zufälliges Zitat von http://bash.org zurück")]
        public async Task Bash(CommandContext ctx)
        {
            var res = Logic.Bash.GetBash();
            await ctx.Channel.SendMessageAsync("\n" + res).ConfigureAwait(false);
        }

        [Command("GBash")]
        [Description("Gibt ein zufälliges Zitat von http://german-bash.org zurück")]
        public async Task GBash(CommandContext ctx)
        {
            var res = Logic.Bash.GetGermanBash();
            await ctx.Channel.SendMessageAsync("\n" + res).ConfigureAwait(false);
        }
    }
}
