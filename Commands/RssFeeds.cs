using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class RssFeeds : BaseCommandModule
    {
        [Command("fefe")]
        [Aliases("effe")]
        [Description("Letzte Fefe-Artikel ")]
        public async Task Fefe(CommandContext ctx, int anz = 1)
        {
            string title = string.Empty;
            if (anz > 3)
            {
                anz = 3;
            }
            var temp = await Shared.GetNewsFeed("https://blog.fefe.de/rss.xml");
            if (temp.Count < anz)
            {
                anz = temp.Count;
            }

            for (int i = 0; i <= anz - 1; i++)
            {
                var item = temp[i];
                if (item.Title.Length > 2048)
                {
                    title = item.Title[..2045] + "...";
                }
                var embed = new DiscordEmbedBuilder
                {
                    Title = item.Id,
                    Description = title,
                    Color = new DiscordColor(0xFF0000)
                };
                await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
            }
        }
    }
}
