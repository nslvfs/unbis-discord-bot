using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.IO;
using System.Text.RegularExpressions;
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
        public async Task Sani(CommandContext ctx, [RemainingText] string qry)
        {
            if (Bot.CheckBadWords(qry))
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": nope").ConfigureAwait(false);
                return;
            }
            var sanitizer = new Ganss.XSS.HtmlSanitizer();
            sanitizer.AllowDataAttributes = false;
            qry = sanitizer.Sanitize(qry);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + qry).ConfigureAwait(false);
        }

        [Command("bpost")]
        [Description("Einen Artikel posten")]
        public async Task BPost(CommandContext ctx, [RemainingText] string qry)
        {
            if (ctx.Guild.Id != 791393115097137171 || Bot.CheckBadWords(qry))
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": nope").ConfigureAwait(false);
                return;
            }

            string dtNow = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            var contentFile = Bot.configJson.blogContent;
            int rssLineCount = 0;
            int lineCount = 0;
            var rssFile = Bot.configJson.blogRss;

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
                    qry = sanitizer.Sanitize(qry);
                    var username = sanitizer.Sanitize(ctx.Member.DisplayName);
                    qry = StripHTML(qry);
                    qry.Replace("\n", "<br>");
                    qry = qry.Trim();
                    if (qry.Length < 10)
                    {
                        await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Das ist ein Blog und kein Twitter, bisschen mehr kannst wohl schreiben!").ConfigureAwait(false);
                        return;
                    }
                    if ((username.Contains("vfs") || username.Contains("unbi") || username.Contains("unbequem")) && ctx.Member.Id != 134719067016658945)
                    {
                        username = "arschloch";
                    }
                    string outTxt = "<span id='" + lineCount + "'><small><a href='#" + lineCount + "'>#</a> <b>" + dtNow + "</b> von " + username + "</small></span>\n<p>" + qry + "</p>\n<hr />\n";
                    File.WriteAllText(contentFile, outTxt + currentContent);
                    if (File.Exists(rssFile))
                    {
                        var _rssData = new Model.RssData();
                        string newData = "<item>\n";
                        newData = newData + "<title>" + username + ": " + qry + "</title>\n";
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
                    }
                    else
                    {
                        await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": RSS-File nicht gefunden.").ConfigureAwait(false);
                        return;
                    }
                }
                else
                {
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Content-File nicht gefunden.").ConfigureAwait(false);
                    return;
                }
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": https://blog.neuschwabenland.net/#" + lineCount).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + e.Message).ConfigureAwait(false);
            }
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}
