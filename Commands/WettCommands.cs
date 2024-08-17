using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using unbis_discord_bot.Logic;

namespace unbis_discord_bot.Commands
{
    public class WettCommands : BaseCommandModule
    {
        private WettenDassLogic WettLogic { get; set; }
        //private bool wetteActive = false;
        //private string curWettTopic = "";
        private ulong wettmeister = 0;

        [Command("newbet")]
        [Aliases("startbet")]
        [Description("Startet eine neue Wette")]
        public async Task StartNewBet(CommandContext ctx, params string[] args)
        {
            WettLogic ??= new WettenDassLogic();
            if (WettLogic.CurWette.wetteActive)
            {
                await ctx.Channel.SendMessageAsync("Es läuft bereits eine Wette: \"" + WettLogic.CurWette.curWettTopic + "\"").ConfigureAwait(false);
                return;
            }

            WettLogic.CurWette.curWettTopic = string.Join(" ", args);
            if (Bot.CheckBadWords(WettLogic.CurWette.curWettTopic, true))
            {
                await ctx.Channel.SendMessageAsync("Ungültige Wette.").ConfigureAwait(false);
                return;
            }
            wettmeister = ctx.Member.Id;
            await ctx.Channel.SendMessageAsync("Die Wette \"" + WettLogic.CurWette.curWettTopic + "\" wurde gestart. Nimm teil mit !bet <einsatz> <ergebnis ja/nein>. Zum Beispiel: !bet 100 Ja").ConfigureAwait(false);
            WettLogic.CurWette.wetteActive = true;
        }

        [Command("endbet")]
        [Description("Startet eine neue Wette")]
        public async Task EndBet(CommandContext ctx, string result)
        {
            if (!WettLogic.CurWette.wetteActive)
            {
                await ctx.Channel.SendMessageAsync("Es läuft derzeit keine Wette.").ConfigureAwait(false);
                return;
            }
            if (wettmeister != ctx.Member.Id)
            {
                await ctx.Channel.SendMessageAsync("Du hast die Wette nicht gestartet.").ConfigureAwait(false);
                return;
            }
            result = result.ToLower();
            if (result != "ja" && result != "nein")
            {
                await ctx.Channel.SendMessageAsync("Kein Gültiges Ergebnis. Ergebis kann nur Ja oder Nein sein").ConfigureAwait(false);
                return;
            }

            WettLogic.CurWette.wetteActive = false;
            await ctx.Channel.SendMessageAsync("Die Wette \"" + WettLogic.CurWette.curWettTopic + "\" wurde mit dem Ergebnis \"" + result + "\" beendet.").ConfigureAwait(false);
        }

        [Command("bet")]
        [Description("Biete auf eine laufende Wette")]
        public async Task Bet(CommandContext ctx, long amount, string janein)
        {
            if (!WettLogic.CurWette.wetteActive)
            {
                await ctx.Channel.SendMessageAsync("Es läuft derzeit keine Wette.").ConfigureAwait(false);
                return;
            }
            if (wettmeister == ctx.Member.Id)
            {
                await ctx.Channel.SendMessageAsync("Als Eröffner der Wette darfst du nicht teilnehmen.").ConfigureAwait(false);
                return;
            }

        }

    }
}
