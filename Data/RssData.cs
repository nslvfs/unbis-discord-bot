using System;
using System.Collections.Generic;
using System.Text;

namespace unbis_discord_bot.Data
{
    public class RssData
    {
        public string headerDaten = @"<?xml version='1.0' encoding='UTF-8'?>
<rss version = '2.0'>
<channel>
<title>Neuschwabenlandblog</title>
<link>https://blog.neuschwabenland.net</link>
<description>Der offizielle Neuschwabenlandblog</description>
<language>de</language>
";

        public string footerDaten = @"</channel>
</rss>";
    }
}
