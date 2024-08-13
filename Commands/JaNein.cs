using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class JaNein : BaseCommandModule
    {

        [Command("rauchen")]
        [Description("Jetzt ein Kippchen?")]
        public async Task Rauchen(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Ja").ConfigureAwait(false);
        }

        [Command("jn")]
        [Description("Beantwortet eine Ja/Nein Frage")]
        public async Task NeinJa(CommandContext ctx)
        {
            var res = Shared.GenerateRandomNumber(1, 100);
            if (res <= 50)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Nein").ConfigureAwait(false);
            }
            else if (res <= 98)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Ja").ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Keine Ahnung, halts Maul").ConfigureAwait(false);
            }
        }

        [Command("pick")]
        [Description("Wählt aus einer Liste von Elementen ein zufälliges aus")]
        public async Task Pick(CommandContext ctx, params string[] args)
        {
            foreach (var arg in args)
                if (Bot.CheckBadWords(arg))
                {
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + Bot.ConfigJson.negativAnswer).ConfigureAwait(false);
                    return;
                }
            var i = Shared.GenerateRandomNumber(0, args.Length - 1);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + args[i]).ConfigureAwait(false);
        }
    }
}
