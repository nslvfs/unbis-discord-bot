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

        [Command("arbeiten")]
        [Description("Arbeiten?")]
        public async Task Arbeiten(CommandContext ctx)
        {
            if (ctx.Member.IsOwner)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Nein").ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Ja").ConfigureAwait(false);
            }
        }

        [Command("nappen")]
        [Description("Nappen?")]
        public async Task Nappen(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Ja").ConfigureAwait(false);
        }

        [Command("feierabend")]
        [Description("Feierabend?")]
        public async Task Feierabend(CommandContext ctx)
        {
            if (ctx.Member.IsOwner)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Ja").ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Nein").ConfigureAwait(false);
            }
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
            var i = Shared.GenerateRandomNumber(0, args.Length);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": "+ args[i]);
        }
    }
}
