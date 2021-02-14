using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading;
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
            var userList = Shared.GetActiveUsersAsync(ctx);
            var rnd = Shared.GenerateRandomNumber(0, userList.Count - 1);
            var picked = ((DiscordMember)userList[rnd]).DisplayName;
            var text = DSharpPlus.Formatter.Italic("dreht die Flasche huiiii");
            Thread.Sleep(1000 * 2); // 2 Sek
            await ctx.Channel.SendMessageAsync(text).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Die Flasche zeigt auf: " + ((DiscordMember)userList[rnd]).Mention).ConfigureAwait(false);
        }
    }
}
