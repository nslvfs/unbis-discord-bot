using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using GoogleApi;
using GoogleApi.Entities.Search.Web.Request;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class Gurgle : BaseCommandModule
    {
        [Command("google")]
        [Aliases("g")]
        [Description("google halt du depp")]
        public async Task Google(CommandContext ctx, [RemainingText] string qry)
        {
            if (!Bot.googlemode && !ctx.Member.IsOwner)
            {
                await ctx.Channel.SendMessageAsync("nope").ConfigureAwait(false);
                return;
            }
            var request = new WebSearchRequest
            {
                Key = Bot.configJson.gurgleApi,
                Query = qry,
                SearchEngineId = Bot.configJson.gurgleSeachEngineId
            };
            var response = await GoogleSearch.WebSearch.QueryAsync(request);

            int i = 0;
            foreach (var resultItem in response.Items)
            {
                i++;
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ":\n" + resultItem.FormattedUrl);
                if (i == 3) break;
            }

        }

        [Command("gbild")]
        [Aliases("gb")]
        [Description("google bild halt du depp")]
        public async Task GoogleBild(CommandContext ctx, [RemainingText] string qry)
        {
            if (!Bot.googlemode && !ctx.Member.IsOwner)
            {
                await ctx.Channel.SendMessageAsync("nope").ConfigureAwait(false);
                return;
            }
            var request = new GoogleApi.Entities.Search.Image.Request.ImageSearchRequest
            {
                Key = Bot.configJson.gurgleApi,
                Query = qry,
                SearchEngineId = Bot.configJson.gurgleSeachEngineId
            };
            var response = await GoogleSearch.WebSearch.QueryAsync(request);

            int i = 0;
            foreach (var resultItem in response.Items)
            {
                i++;
                if (!resultItem.Link.ToString().StartsWith("x-raw-image"))
                {
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ":\n" + resultItem.Link);
                }
                else
                {
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ":\n" + resultItem.Image.ThumbnailLink);
                }
                if (i == 3) break;
            }

        }
    }
}
