using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using unbis_discord_bot.Logic;

namespace unbis_discord_bot.Commands
{
    public class Arbeitszeiten : BaseCommandModule
    {
        [Command("arbeit")]
        [Description("Fragt die Arbeitstage für ein User ab")]
        public async Task Arbeit(CommandContext ctx, DiscordUser target, string day = "")
        {
            if (day.Length == 2)
            {
                day = char.ToUpper(day[0]) + day.Substring(1).ToLower();
            }
            string[] tage = { "Mo", "Di", "Mi", "Do", "Fr", "Sa", "So" };
            if (day != "")
            {
                int i = Array.IndexOf(tage, day);
                {
                    var outTxt = LoArbeitszeiten.GetSingleDay(target.Id, tage[i]);
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + target.Mention + " " + outTxt).ConfigureAwait(false);
                }
            }
            else
            {
                foreach (var tag in tage)
                {
                    var outTxt = LoArbeitszeiten.GetSingleDay(target.Id, tag);
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + target.Mention + " " + outTxt).ConfigureAwait(false);
                    if (outTxt == "keine Daten Vorhanden")
                    {
                        break;
                    }
                }
            }
        }

        [Command("setarbeit")]
        [Description("Fragt die Arbeitstage für ein User ab")]
        public async Task SetArbeit(CommandContext ctx, string day, string begin, string end)
        {
            if (day.Length == 2)
            {
                day = char.ToUpper(day[0]) + day.Substring(1).ToLower();
            }
            string[] tage = { "Mo", "Di", "Mi", "Do", "Fr", "Sa", "So" };
            var temp = LoArbeitszeiten.ValidateInput(begin, end);
            if (!temp)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Eingabe ungültig").ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Gültige Tage: Mo, Di, Mi, Do, Fr, Sa, So").ConfigureAwait(false);
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
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": o7").ConfigureAwait(false);
        }

        [Command("delarbeit")]
        [Description("Fragt die Arbeitstage für ein User ab")]
        public async Task DelArbeit(CommandContext ctx, string day)
        {
            if (day.Length == 2)
            {
                day = char.ToUpper(day[0]) + day.Substring(1).ToLower();
            }
            string[] tage = { "Mo", "Di", "Mi", "Do", "Fr", "Sa", "So" };
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
            else
            {
                LoArbeitszeiten.DelSingleDay(ctx.Member.Id, day);
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": o7").ConfigureAwait(false);
            }
        }

        [Command("delallarbeit")]
        [Description("Löscht die Arbeitswoche")]
        public async Task DelAllArbeit(CommandContext ctx)
        {
            LoArbeitszeiten.DelWeek(ctx.Member.Id);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": o7").ConfigureAwait(false);

        }

        [Command("setallarbeit")]
        [Description("Löscht die Arbeitswoche")]
        public async Task SetAllDays(CommandContext ctx, string begin, string end)
        {
            string[] tage = { "Mo", "Di", "Mi", "Do", "Fr", "Sa", "So" };
            var temp = LoArbeitszeiten.ValidateInput(begin, end);
            if (!temp)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Eingabe ungültig").ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Gültige Zeiten z.B.: 6:12, 20:15").ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Beispielerhafter Aufruf:").ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": !setarbeit Mo 10:00 18:30").ConfigureAwait(false);
                return;
            }

            TimeSpan beginTs = TimeSpan.Parse(begin);
            TimeSpan endTs = TimeSpan.Parse(end);
            foreach(var tag in tage)
            {
                LoArbeitszeiten.SetSingleDay(ctx.User.Id, tag, beginTs, endTs);
            }
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": o7").ConfigureAwait(false);
        }
    }
}
