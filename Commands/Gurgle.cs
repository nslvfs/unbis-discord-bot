using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using GScraper;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class Gurgle : BaseCommandModule
    {
        [Command("gbild")]
        [Aliases("gb")]
        [Description("google bild halt du depp")]
        public async Task GoogleBild(CommandContext ctx, [RemainingText] string qry)
        {
            if (Bot.checkBadWords(qry))
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": nope").ConfigureAwait(false);
                return;
            }
            if (ctx.Channel.Id != 816990123568660510)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": nope").ConfigureAwait(false);
                return;
            }
            int limit = 3;
            var scraper = new GoogleScraper();
            var images = await scraper.GetImagesAsync(qry, limit).ConfigureAwait(false);
            foreach (var image in images)
            {
                string result = "";
                if (!image.Link.StartsWith("x-raw-image"))
                {
                    result = image.Link;
                }
                else
                {
                    result = image.ThumbnailLink;
                }
                if (!Bot.checkBadWords(result))
                {
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + image.Link).ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": nope").ConfigureAwait(false);
                }
            }

        }
    }
}
