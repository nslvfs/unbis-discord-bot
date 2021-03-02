using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class RussischRoulette : BaseCommandModule
    {
        private static List<Model.PerChannelRoulette> rouletteData = new List<Model.PerChannelRoulette>();

        SemaphoreSlim _semaphoregate = new SemaphoreSlim(1, 1);

        [Command("roulette")]
        [Aliases("r")]
        [Description("Russischer Familienspaß")]
        public async Task Roulette(CommandContext ctx)
        {
            try
            {
                int index = 0;
                await _semaphoregate.WaitAsync();
                if (ctx.Channel.Id == 812403060416446474) //kiosk
                {
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Nicht hier").ConfigureAwait(false);
                    _semaphoregate.Release();
                    return;
                }
                var found = false;
                foreach (var item in rouletteData)
                {
                    if (item.channelId == ctx.Channel.Id)
                    {
                        found = true;
                        break;
                    }
                    index++;
                }
                if (!found)
                {
                    Model.PerChannelRoulette newItem = new Model.PerChannelRoulette();
                    newItem.channelId = ctx.Channel.Id;
                    newItem.revShots = -1;
                    rouletteData.Add(newItem);
                }
                await RouletteLogic(ctx, rouletteData.Count - 1);
                _semaphoregate.Release();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task RouletteLogic(CommandContext ctx, int index)
        {
            if (rouletteData[index].channelId == ctx.Channel.Id)
            {
                if (rouletteData[index].revKammer == -1)
                {
                    var text = DSharpPlus.Formatter.Italic("lädt den Revolver");
                    await ctx.Channel.SendMessageAsync(text).ConfigureAwait(false);
                    rouletteData[index].revKammer = Shared.GenerateRandomNumber(1, 8);
                    rouletteData[index].revShots = 0;
                }
                rouletteData[index].revShots++;
                if (rouletteData[index].revShots != rouletteData[index].revKammer)
                {
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": ** klick **").ConfigureAwait(false);
                }
                if (rouletteData[index].revShots == rouletteData[index].revKammer)
                {
                    rouletteData[index].revKammer = -1;
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": ** BOOM **").ConfigureAwait(false);
                    _semaphoregate.Release();
                    if (ctx.Guild.Id == 791393115097137171)
                    {
                        _ = Bot.Mute(ctx.Message, ctx.Guild);
                    }
                }
            }


        }
    }
}
