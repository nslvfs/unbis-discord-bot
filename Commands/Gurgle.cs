using System;
using System.Collections.Generic;
using System.Text;
using GoogleApi;
using GoogleApi.Entities.Search.Common;
using DSharpPlus.CommandsNext;
using GoogleApi.Entities.Search.Web.Request;
using DSharpPlus.CommandsNext.Attributes;
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
            var request = new WebSearchRequest
            {
                Key = Bot.configJson.gurgleApi,
                Query = qry,
                SearchEngineId = "6af40d513c97bb21f"

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
    }
}
