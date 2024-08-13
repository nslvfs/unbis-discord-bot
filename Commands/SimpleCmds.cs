using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class SimpleCmds : BaseCommandModule
    {
        [Command("fork")]
        [Aliases("github")]
        [Description("Jetzt ein Kippchen?")]
        public async Task Fork(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": https://github.com/nslvfs/unbis-discord-bot").ConfigureAwait(false);
        }

        [Command("getRandomNumber()")]
        [Description("Generiert eine zufällige Zahl")]
        public async Task GetRandomNumber(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": 9").ConfigureAwait(false);
        }


        [Command("w20")]
        [Description("Wirft einen W20")]
        public async Task W20(CommandContext ctx)
        {
            var res = Shared.GenerateRandomNumber(1, 20);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + res).ConfigureAwait(false);
        }

        [Command("dice")]
        [Description("Wirft einen Würfel mit x Seiten")]
        public async Task Dice(CommandContext ctx, short num)
        {
            var res = Shared.GenerateRandomNumber(1, num);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + res).ConfigureAwait(false);
        }

    }
}
