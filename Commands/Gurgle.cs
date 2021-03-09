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
            if (Bot.CheckBadWords(qry) || ctx.Channel.Id != Bot.channelIdRotz)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + Bot.configJson.negativAnswer).ConfigureAwait(false);
                return;
            }
            int limit = 3;
            var scraper = new GoogleScraper();
            var images = await scraper.GetImagesAsync(qry, limit).ConfigureAwait(false);
            foreach (var image in images)
            {
                string result = string.Empty;
                if (!image.Link.StartsWith("x-raw-image"))
                {
                    result = image.Link;
                }
                else
                {
                    result = image.ThumbnailLink;
                }
                if (!Bot.CheckBadWords(result))
                {
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + image.Link).ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + Bot.configJson.negativAnswer).ConfigureAwait(false);
                }
            }

        }
    }
}
