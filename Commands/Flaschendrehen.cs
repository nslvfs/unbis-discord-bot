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
        public async Task Spin(CommandContext ctx)
        {

            var userList = await Shared.GetActiveUsers(ctx);

            var rnd = Shared.GenerateRandomNumber(0, userList.Count);
            var picked = ((DiscordMember)userList[rnd]).DisplayName;

            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": der ausgewählte Zufallsuser ist: " + ((DiscordMember)userList[rnd]).Mention).ConfigureAwait(false);

        }


        [Command("active")]
        [Description("Vom Bot als aktiv erkannte User")]
        public async Task ActiveUser(CommandContext ctx)
        {
            var userList = await Shared.GetActiveUsers(ctx);

            string result = "Aktive Nutzer: ";
            foreach (var user in userList)
            {
                var name = ((DiscordMember)user).DisplayName;
                result = result + name + ", ";
            }
            result = result.Substring(0, result.Length - 2);
            await ctx.Channel.SendMessageAsync(result).ConfigureAwait(false);
        }
    }
}
