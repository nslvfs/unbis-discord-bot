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
        [Description("Klassisches Flaschendrehen")]
        public async Task Spin(CommandContext ctx)
        {
            var userList = Shared.GetActiveUsers(ctx);
            var rnd = Shared.GenerateRandomNumber(0, userList.Count - 1);
            var text = DSharpPlus.Formatter.Italic("dreht die Flasche huiiii");
            await ctx.Channel.SendMessageAsync(text).ConfigureAwait(false);
            Thread.Sleep(1000 * 2);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Die Flasche zeigt auf: " + ((DiscordMember)userList[rnd]).Mention).ConfigureAwait(false);
        }
    }
}
