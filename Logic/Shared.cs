using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml;

namespace unbis_discord_bot
{
    public class Shared
    {
        public static int GenerateRandomNumber(int min, int max)
        {
            RNGCryptoServiceProvider c = new RNGCryptoServiceProvider();
            byte[] randomNumber = new byte[4];
            c.GetBytes(randomNumber);
            int result = Math.Abs(BitConverter.ToInt32(randomNumber, 0));
            return result % (max - min + 1) + min;
        }

        public static List<DiscordUser> GetActiveUsers(CommandContext ctx)
        {
            List<DiscordUser> userList = new List<DiscordUser>();

            foreach (var msg in Bot.ArchivMessages)
            {
                if (msg.ChannelId == ctx.Channel.Id)
                {
                    if (userList.Count > 0)
                    {
                        var found = false;
                        foreach (var auth in userList)
                        {
                            if (auth.Id == msg.Author.Id)
                            {
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            userList.Add(msg.Author);
                        }
                    }
                    else
                    {
                        userList.Add(msg.Author);
                    }
                }
            }
            return userList;
        }

        public static string GetRedirect(string url)
        {
            using (var client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }))
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get
                };

                HttpResponseMessage response = client.SendAsync(request).Result;
                return response.RequestMessage.RequestUri.ToString();
            }

        }

        public static async Task<List<ISyndicationItem>> GetNewsFeed(string feedUri)
        {
            var rssNewsItems = new List<ISyndicationItem>();
            using (var xmlReader = XmlReader.Create(feedUri, new XmlReaderSettings() { Async = true }))
            {
                var feedReader = new RssFeedReader(xmlReader);
                while (await feedReader.Read())
                {
                    if (feedReader.ElementType == SyndicationElementType.Item)
                    {
                        ISyndicationItem item = await feedReader.ReadItem();
                        rssNewsItems.Add(item);
                    }
                }
            }
            return rssNewsItems;
        }
    }
}
