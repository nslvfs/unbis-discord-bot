﻿using DSharpPlus.CommandsNext;
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
            if(anz > 3)
            {
                anz = 3;
            }
            var temp = await Shared.GetNewsFeed("https://blog.fefe.de/rss.xml");
            if(temp.Count < anz)
            {
                anz = temp.Count;
            }

            for(int i = 0; i <= anz -1; i++) {
                var item = temp[i];
                if(item.Title.Length > 2048)
                {
                    item.Title.Substring(0, 2048);
                }
                var embed = new DiscordEmbedBuilder
                {
                    Title = item.Id,
                    Description = item.Title,
                    Color = new DiscordColor(0xFF0000)
                };
                await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
            }
        }

        [Command("spiegel")]
        [Description("Letzte Spiegel-Topmledung ")]
        public async Task Spiegel(CommandContext ctx, int anz = 1)
        {
            if (anz > 3)
            {
                anz = 3;
            }
            var temp = await Shared.GetNewsFeed("https://www.spiegel.de/schlagzeilen/tops/index.rss");
            if (temp.Count < anz)
            {
                anz = temp.Count;
            }

            for (int i = 0; i <= anz - 1; i++)
            {
                var item = temp[i];
                if (item.Title.Length > 2048)
                {
                    item.Title.Substring(0, 2048);
                }
                System.Uri imgLink = null;
                foreach (var link in item.Links)
                {
                    if(link.MediaType == "image/jpeg")
                    {
                        imgLink = link.Uri;
                        break;
                    }
                }
                var embed = new DiscordEmbedBuilder
                {
                    Title = item.Title + " " + item.Id,
                    ImageUrl = imgLink.ToString(),
                    Description = item.Description,
                    Color = new DiscordColor(0xFF0000)
                };
                await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
            }
        }


        [Command("spiegeleil")]
        [Description("Letzte Spiegel-Eilmeldungen ")]
        public async Task SpiegelEil(CommandContext ctx, int anz = 1)
        {
            if (anz > 3)
            {
                anz = 3;
            }
            var temp = await Shared.GetNewsFeed("https://www.spiegel.de/schlagzeilen/eilmeldungen/index.rss");
            if (temp.Count < anz)
            {
                anz = temp.Count;
            }
            if(temp.Count == 0)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Derzeit keine Eilmeldungen").ConfigureAwait(false);
                return;
            }

            for (int i = 0; i <= anz - 1; i++)
            {
                var item = temp[i];
                if (item.Title.Length > 2048)
                {
                    item.Title.Substring(0, 2048);
                }
                System.Uri imgLink = null;
                foreach (var link in item.Links)
                {
                    if (link.MediaType == "image/jpeg")
                    {
                        imgLink = link.Uri;
                        break;
                    }
                }
                var embed = new DiscordEmbedBuilder
                {
                    Title = item.Title + " " + item.Id,
                    ImageUrl = imgLink.ToString(),
                    Description = item.Description,
                    Color = new DiscordColor(0xFF0000)
                };
                await ctx.Channel.SendMessageAsync(embed).ConfigureAwait(false);
            }
        }
    }
}
