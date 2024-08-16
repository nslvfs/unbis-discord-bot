using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.XPath;
using unbis_discord_bot.Logic;

namespace unbis_discord_bot.Commands
{
    public class WettCommands : BaseCommandModule
    {
        private WettenDassLogic WettLogic { get; set; }
        private bool wetteActive = false;
        private string curWettTopic = "";
        private ulong wettmeister = 0;

        [Command("newbet")]
        [Aliases("startbet")]
        [Description("Startet eine neue Wette")]
        public async Task StartNewBet(CommandContext ctx, params string[] args)
        {
            if(wetteActive)
            {
                await ctx.Channel.SendMessageAsync("Es läuft bereits eine Wette: \"" + curWettTopic + "\"").ConfigureAwait(false);
                return;
            }
            
            WettLogic ??= new WettenDassLogic();
            curWettTopic = string.Join(" ", args);
            if (Bot.CheckBadWords(curWettTopic, true))
            {
                await ctx.Channel.SendMessageAsync("Ungültige Wette.").ConfigureAwait(false);
                return;
            }
            wettmeister = ctx.Member.Id;
            await ctx.Channel.SendMessageAsync("Die Wette \"" + curWettTopic + "\" wurde gestart. Nimm teil mit !bet <einsatz> <ergebnis ja/nein>. Zum Beispiel: !bet 100 Ja").ConfigureAwait(false);
            wetteActive = true;
        }

        [Command("endbet")]
        [Description("Startet eine neue Wette")]
        public async Task EndBet(CommandContext ctx, string result)
        {
            if (!wetteActive)
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
            if(result != "ja" && result != "nein")
            {
                await ctx.Channel.SendMessageAsync("Kein Gültiges Ergebnis. Ergebis kann nur Ja oder Nein sein").ConfigureAwait(false);
                return;
            }

            wetteActive = false;
            await ctx.Channel.SendMessageAsync("Die Wette \""+curWettTopic+"\" wurde mit dem Ergebnis \""+ result +"\" beendet.").ConfigureAwait(false);
        }

        [Command("bet")]
        [Description("Biete auf eine laufende Wette")]
        public async Task Bet(CommandContext ctx, long amount, string janein)
        {
            if (!wetteActive)
            {
                await ctx.Channel.SendMessageAsync("Es läuft derzeit keine Wette.").ConfigureAwait(false);
                return;
            }
            if(wettmeister == ctx.Member.Id)
            {
                await ctx.Channel.SendMessageAsync("Als Eröffner der Wette darfst du nicht teilnehmen.").ConfigureAwait(false);
                return;
            }
            
        }

    }
}
