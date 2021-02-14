using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;

namespace unbis_discord_bot
{
    public class Shared
    {
        public static int GenerateRandomNumber(int min, int max)
        {
            RNGCryptoServiceProvider c = new RNGCryptoServiceProvider();
            // Ein integer benötigt 4 Byte
            byte[] randomNumber = new byte[4];
            // dann füllen wir den Array mit zufälligen Bytes
            c.GetBytes(randomNumber);
            // schließlich wandeln wir den Byte-Array in einen Integer um
            int result = Math.Abs(BitConverter.ToInt32(randomNumber, 0));
            // da bis jetzt noch keine Begrenzung der Zahlen vorgenommen wurde,
            // wird diese Begrenzung mit einer einfachen Modulo-Rechnung hinzu-
            // gefügt
            return result % (max - min + 1) + min; //fix by opi
        }

        public static List<DiscordUser> GetActiveUsers(CommandContext ctx)
        {
            var messages = Bot.ArchivMessages;
            var channelID = ctx.Channel.Id;
            List<DiscordUser> userList = new List<DiscordUser>();

            foreach (var msg in messages)
            {
                if(msg.ChannelId == channelID) { 
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
    }
}
