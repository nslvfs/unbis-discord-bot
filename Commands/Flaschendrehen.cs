using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class Flaschendrehen : BaseCommandModule
    {
        [Command("spin")]
        [Aliases("wichsen")]
        [Description("Klassisches Flaschendrehen")]
        public async Task Banshee(CommandContext ctx)
        {   

            var userList = await GetActiveUsers(ctx);

            var rnd = Shared.GenerateRandomNumber(0, userList.Count);
            var picked = ((DiscordMember)userList[rnd]).DisplayName;

            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": der ausgewählte Zufallsuser ist: " + ((DiscordMember)userList[rnd]).Mention).ConfigureAwait(false);

        }


        [Command("active")]
        [Description("Vom Bot als aktiv erkannte User")]
        public async Task ActiveUser(CommandContext ctx)
        {
            var messages = await ctx.Channel.GetMessagesAsync(100);
            var userList = await GetActiveUsers(ctx);

            string result = "Aktive Nutzer: ";
            foreach(var user in userList)
            {
                var name = ((DiscordMember)user).DisplayName;
                result = result + name + ", ";
            }
            result = result.Substring(0, result.Length - 2);
            await ctx.Channel.SendMessageAsync(result).ConfigureAwait(false);
        }


        private async Task<List<DiscordUser>> GetActiveUsers(CommandContext ctx)
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
