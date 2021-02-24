﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class NslBlog : BaseCommandModule
    {
        [Command("blog")]
        [Description("Link zum Blog")]
        public async Task Blog(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": https://blog.neuschwabenland.net").ConfigureAwait(false);
        }

        [Command("sani")]
        [Description("sanitzer test")]
        public async Task Sani(CommandContext ctx, params string[] args)
        {
            string result = string.Empty;
            foreach (var text in args)
            {
                result = result + " " + text;
            }
            var sanitizer = new Ganss.XSS.HtmlSanitizer();
            sanitizer.AllowDataAttributes = false;
            result = sanitizer.Sanitize(result);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + result).ConfigureAwait(false);
        }


        [Command("bpost")]
        [Description("Einen Artikel posten")]
        public async Task BPost(CommandContext ctx, params string[] args)
        {
            string dtNow = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            var contentFile = Bot.configJson.blogContent;
            int rssLineCount = 0;
            int lineCount = 0;
            var rssFile = Bot.configJson.blogRss;
            string result = string.Empty;
            foreach (var text in args)
            {
                result = result + " " + text;
            }
            try
            {
                if (File.Exists(contentFile))
                {
                    
                    using (var reader = File.OpenText(contentFile))
                    {
                        while (reader.ReadLine() != null)
                        {
                            lineCount++;
                        }
                    }

                    var currentContent = File.ReadAllText(contentFile);

                    var sanitizer = new Ganss.XSS.HtmlSanitizer();
                    sanitizer.AllowDataAttributes = false;
                    result = sanitizer.Sanitize(result);
                    var username = sanitizer.Sanitize(ctx.Member.DisplayName);

                    result.Replace("\n", "<br>");
                    string outTxt = "<span id='" + lineCount + "'><small><a href='#" + lineCount + "'>#</a> <b>" + dtNow + "</b> von " + username + "</small></span>\n<p>" + result + "</p>\n<hr />\n";
                    File.WriteAllText(contentFile, outTxt  + currentContent);
                    if(File.Exists(rssFile))
                    {
                        var _rssData = new Data.RssData();
                        string newData = "<item>\n";
                        newData = newData + "<title>" + username + ": " + result + " </title>\n";
                        newData = newData + "<link>https://blog.neuschwabenland.net/#" + lineCount + "</link>\n";
                        newData = newData + "<guid>https://blog.neuschwabenland.net/#" + lineCount + "</guid>\n";
                        newData = newData + "</item>\n";
                        string fileHeader = _rssData.headerDaten;
                        string fileFooter = _rssData.footerDaten;

                        string fileBody = string.Empty;

                        string[] lines = File.ReadAllLines(rssFile);
                        foreach (string line in lines)
                        {
                            rssLineCount++;
                            if (rssLineCount > 7)
                            {
                                fileBody = fileBody + line + "\n";
                            }
                        }

                        string newFile = fileHeader + newData + "\n" + fileBody;
                        File.WriteAllText(rssFile, newFile);


                    } else
                    {
                        await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": RSS-File nicht gefunden.").ConfigureAwait(false);
                    }
                }
                else
                {
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Content-File nicht gefunden.").ConfigureAwait(false);
                }
                // +
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": https://blog.neuschwabenland.net/#" + lineCount).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + e.Message).ConfigureAwait(false);
            }
        }
    }
}
