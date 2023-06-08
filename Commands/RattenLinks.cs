using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.IO;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class RattenLinks : BaseCommandModule
    {

        [Command("AddGif")]
        [Description("Fügt ein Gif zu !gif hinzu.")]
        public async Task AddGif(CommandContext ctx, [RemainingText] string qry)
        {
            if (Bot.CheckBadWords(qry) || ctx.Guild.Id != Bot.guildIdRatte)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + Bot.ConfigJson.negativAnswer).ConfigureAwait(false);
                return;
            }

            var fileName = Bot.ConfigJson.quotePath + ctx.Guild.Id + " gif.txt";

            Uri uriResult;
            bool result = Uri.TryCreate(qry, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (qry.Length <= 3 || !result)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Das ist sicher kein Link").ConfigureAwait(false);
                return;
            }
            if (!File.Exists(fileName))
            {
                File.Create(fileName).Dispose();
            }

            using (StreamWriter w = File.AppendText(fileName))
            {
                w.WriteLine(qry.Trim());
            }
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Gif hinzugefügt du Pisser!").ConfigureAwait(false);
        }

        [Command("gif")]
        [Description("Gibt ein zufälliges gif wieder.")]
        public async Task Gif(CommandContext ctx)
        {
            if (ctx.Guild.Id != Bot.guildIdRatte)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + Bot.ConfigJson.negativAnswer).ConfigureAwait(false);
                return;
            }
            var temp = new Model.Links(Bot.ConfigJson, ctx.Guild.Id);
            if (temp.links.Count > 0)
            {
                var res = Shared.GenerateRandomNumber(0, temp.links.Count - 1);
                await ctx.Channel.SendMessageAsync(temp.links[res]).ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync("Keine Gifs gefunden :c").ConfigureAwait(false);
            }

        }
    }
}
