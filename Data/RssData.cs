﻿namespace unbis_discord_bot.Model
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
