using System;
using System.Collections.Generic;
using System.Text;
using unbis_discord_bot.Logic;
using unbis_discord_bot.Model;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    class Arbeitszeiten : BaseCommandModule
    {
        [Command("arbeit")]
        [Description("Fragt die Arbeitstage für ein User ab")]
        public async Task Arbeit(CommandContext ctx, DiscordUser target)
        {
            string[] tage = { "Mo", "Di", "Mi", "Do", "Fr", "Sa", "So" };
            foreach (var tag in tage)
            {
                var outTxt = LoArbeitszeiten.GetSingleDay(target.Id, tag);
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + target.Mention + " " + outTxt).ConfigureAwait(false);
            }
        }

        [Description("Fragt einen Arbeitstag für ein User ab")]
        public async Task Arbeit(CommandContext ctx, DiscordUser target, string day)
        {
            string[] tage = { "Mo", "Di", "Mi", "Do", "Fr", "Sa", "So" };
            int i = Array.IndexOf(tage, day);
            {
                var outTxt = LoArbeitszeiten.GetSingleDay(target.Id, tage[i]);
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + target.Mention + " " + outTxt).ConfigureAwait(false);
            }
        }

        [Command("setarbeit")]
        [Description("Fragt die Arbeitstage für ein User ab")]
        public async Task SetArbeit(CommandContext ctx, string day, string begin, string end)
        {
            string[] tage = { "Mo", "Di", "Mi", "Do", "Fr", "Sa", "So" };
            var temp = LoArbeitszeiten.ValidateInput(begin, end);
            if(!temp)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Eingabe ungültig" ).ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Gültige Tage: Mo, Di, Mi, Do, Fr, Sa So").ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Gültige Zeiten z.B.: 6:12, 20:15").ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Beispielerhafter Aufruf:").ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": !setarbeit Mo 10:00 18:30").ConfigureAwait(false);
                return;
            }

            TimeSpan beginTs = TimeSpan.Parse(begin);
            TimeSpan endTs = TimeSpan.Parse(end);

            if (Array.IndexOf(tage, day) == -1)
            {
                string outTxt = "Ungültiger Tag! Gültige Einträge: ";
                foreach (var str in tage)
                {
                    outTxt = outTxt + str + ", ";
                }
                outTxt = outTxt.Substring(0, outTxt.Length - 2);
                Console.WriteLine(outTxt);
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + outTxt).ConfigureAwait(false);
                return;
            }

            LoArbeitszeiten.SetSingleDay(ctx.User.Id, day, beginTs, endTs);

        }
    }
}
