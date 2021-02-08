using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

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
            return result % max + min;
        }

        public static async Task<List<DiscordUser>> GetActiveUsers(CommandContext ctx)
        {
            var messages = await ctx.Channel.GetMessagesAsync(100);

            List<DiscordUser> userList = new List<DiscordUser>();

            foreach (var msg in messages)
            {
                if (!msg.Author.IsBot)
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
    }
}
