using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using NScrape;
using WebRequest = System.Net.WebRequest;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text;
using GScraper;
using System.Collections.Generic;

namespace unbis_discord_bot.Commands
{
    public class Gurgle : BaseCommandModule
    {
        [Command("google")]
        [Aliases("g")]
        [Description("google halt du depp")]
        public async Task Google(CommandContext ctx, [RemainingText] string qry)
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
            StringBuilder bufferForHtml = new StringBuilder();
            byte[] encodedBytes = new byte[8192];
            var urlForSearch = "https://google.com/search?q=" + qry.Trim();
            var request = (HttpWebRequest)WebRequest.Create(urlForSearch);
            var response = (HttpWebResponse)request.GetResponse();

            using (Stream responseFromGoogle = response.GetResponseStream())
            {
                var enc = response.GetEncoding();

                int count = 0;
                do
                {
                    count = responseFromGoogle.Read(encodedBytes, 0, encodedBytes.Length);
                    if (count != 0)
                    {
                        var tempString = enc.GetString(encodedBytes, 0, count);
                        bufferForHtml.Append(tempString);
                    }
                }

                while (count > 0);
            }
            string sbb = bufferForHtml.ToString();

            var processedHtml = new HtmlAgilityPack.HtmlDocument
            {
                OptionOutputAsXml = true
            };
            processedHtml.LoadHtml(sbb);
            var doc = processedHtml.DocumentNode;
            int i = 0;
            foreach (var link in doc.SelectNodes("//a[@href]"))
            {
                
                string hrefValue = link.GetAttributeValue("href", string.Empty);
                if (!hrefValue.ToUpper().Contains("GOOGLE") && hrefValue.Contains("/url?q=") && hrefValue.ToUpper().Contains("HTTP"))
                {
                    int index = hrefValue.IndexOf("&");
                    if (index > 0)
                    {
                        i++;
                        hrefValue = hrefValue.Substring(0, index);
                        await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ":\n" + hrefValue.Replace("/url?q=", string.Empty)).ConfigureAwait(false);
                        if (i == 3) break;
                    }
                }
                
            }
        }

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
                if (!image.Link.StartsWith("x-raw-image")) {
                    result = image.Link;
                } else
                {
                    result = image.ThumbnailLink;
                }
                if(!Bot.checkBadWords(result)) { 
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + image.Link).ConfigureAwait(false);
                } else
                {
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": nope").ConfigureAwait(false);
                }
            }

        }
    }
}
