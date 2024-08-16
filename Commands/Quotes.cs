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
            var temp = new Model.Quotes(Bot.ConfigJson, ctx.Guild.Id);
            if (temp.quotes.Count > 0)
            {
                var res = Shared.GenerateRandomNumber(0, temp.quotes.Count - 1);
                if (ctx.Guild.Id == Bot.guildIdRatte)
                {
                    await ctx.Channel.SendMessageAsync("Kevin sagt: " + temp.quotes[res]).ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendMessageAsync(temp.quotes[res]).ConfigureAwait(false);
                }
            }
            else
            {
                await ctx.Channel.SendMessageAsync("Keine Quotes gefunden :c").ConfigureAwait(false);
            }
        }

        [Command("Addquote")]
        [Description("Fügt ein Zitat zu !quote hinzu.")]
        public async Task QuoteAdd(CommandContext ctx, [RemainingText] string qry)
        {
            if (Bot.CheckBadWords(qry))
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + Bot.ConfigJson.negativAnswer).ConfigureAwait(false);
                return;
            }

            var fileName = Bot.ConfigJson.quotePath + ctx.Guild.Id + ".txt";
            if (qry.Length <= 3)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Das ist kein Quote").ConfigureAwait(false);
                return;
            }
            qry = qry + " (von " + ctx.Member.Mention + " hinzugefügt)";

            if (!File.Exists(fileName))
            {
                File.Create(fileName).Dispose();
            }

            using (StreamWriter w = File.AppendText(fileName))
            {
                w.WriteLine(qry.Trim());
            }
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Quote hinzugefügt du Pisser!").ConfigureAwait(false);
        }



    }
}
