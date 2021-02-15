using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class RussischRoulette : BaseCommandModule
    {
        private static List<Model.PerChannelRoulette> rouletteData = new List<Model.PerChannelRoulette>();

        [Command("roulette")]
        [Aliases("r")]
        [Description("Russischer Familienspaß")]
        public async Task Roulette(CommandContext ctx)
        {
            var found = false;
            foreach (var item in rouletteData)
            {
                if (item.channelId == ctx.Channel.Id)
                {
                    found = true;
                    if (item.revKammer == -1)
                    {
                        var text = DSharpPlus.Formatter.Italic("lädt den Revolver");
                        await ctx.Channel.SendMessageAsync(text).ConfigureAwait(false);
                        item.revKammer = Shared.GenerateRandomNumber(1, 8);
                        item.revShots = 0;
                    }
                    item.revShots++;
                    if (item.revShots != item.revKammer)
                    {
                        await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": ** klick **").ConfigureAwait(false);
                    }
                    if (item.revShots == item.revKammer)
                    {
                        item.revKammer = -1;
                        await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": ** BOOM **").ConfigureAwait(false);
                        if (ctx.Guild.Id == 791393115097137171) { 
                            var roleMuted = ctx.Guild.GetRole(807921762570469386);
                            await ctx.Member.GrantRoleAsync(roleMuted);
                            Thread.Sleep(1000 * 60 * 10); // 10 Min
                            await ctx.Member.RevokeRoleAsync(roleMuted);
                            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + " jetzt nicht mehr gemuted").ConfigureAwait(false);
                        }
                    }
                    break;
                }
            }
            if (!found)
            {
                Model.PerChannelRoulette newItem = new Model.PerChannelRoulette();
                newItem.channelId = ctx.Channel.Id;
                rouletteData.Add(newItem);
                await Roulette(ctx);
            }

        }
    }
}
