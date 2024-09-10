using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using unbis_discord_bot.Logic;
using unbis_discord_bot.Model;

namespace unbis_discord_bot.Commands
{
    public class WettCommands : BaseCommandModule
    {
        private WettenDassLogic WettLogic { get; set; }

        [Command("newbet")]
        [Aliases("startbet")]
        [Description("Startet eine neue Wette")]
        public async Task StartNewBet(CommandContext ctx, params string[] args)
        {
            WettLogic ??= new WettenDassLogic();
            WettLogic.TokenGiveOut();

            if (WettLogic.CurWette.wetteActive)
            {
                await ctx.Channel.SendMessageAsync("Es läuft bereits eine Wette: \"" + WettLogic.CurWette.curWettTopic + "\"").ConfigureAwait(false);
                return;
            }

            var wettTopic = string.Join(" ", args);
            if (Bot.CheckBadWords(WettLogic.CurWette.curWettTopic, true))
            {
                await ctx.Channel.SendMessageAsync("Ungültige Wette.").ConfigureAwait(false);
                return;
            }

            WettLogic.CurWette = new Wette();
            WettLogic.CurWette.curWettTopic = wettTopic;
            WettLogic.CurWette.UserIdStartedBet = ctx.Member.Id;
            WettLogic.CurWette.BetStarted = DateTime.Now;
            await ctx.Channel.SendMessageAsync("Die Wette \"" + WettLogic.CurWette.curWettTopic + "\" wurde gestart. Nimm teil mit !bet <einsatz> <ergebnis ja/nein>. Zum Beispiel: !bet 100 Ja").ConfigureAwait(false);
            WettLogic.CurWette.wetteActive = true;
        }

        [Command("endbet")]
        [Description("Startet eine neue Wette")]
        public async Task EndBet(CommandContext ctx, string result)
        {
            WettLogic ??= new WettenDassLogic();
            WettLogic.TokenGiveOut();

            if (!WettLogic.CurWette.wetteActive)
            {
                await ctx.Channel.SendMessageAsync("Es läuft derzeit keine Wette.").ConfigureAwait(false);
                return;
            }

            bool bypass = (DateTime.Now - WettLogic.CurWette.BetStarted).Days >= 1;

            if (WettLogic.CurWette.UserIdStartedBet != ctx.Member.Id && bypass == false)
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
            string outTxt = string.Empty;
            WettLogic.CurWette.wetteActive = false;
            outTxt += "Die Wette \"" + WettLogic.CurWette.curWettTopic + "\" wurde mit dem Ergebnis \"" + result + "\" beendet.\n";
            var gewinners = WettLogic.CurWette.WettEinsaetze.Where(x => x.Vote == result).ToList();
            var userBonus = WettLogic.DbData.SingleOrDefault(x => x.id == WettLogic.CurWette.UserIdStartedBet);
            userBonus.tokenBalance += WettLogic.CurWette.DealerCut;
            outTxt += "Der Wettstarter erhält eine Sonderzahlung von " + WettLogic.CurWette.DealerCut + " Token\n";

            foreach (var gewinner in gewinners)
            {
                var user = WettLogic.DbData.FirstOrDefault(x => x.id == gewinner.UserId);
                var multiplikator = WettLogic.CurWette.getOddsYes;
                if (result == "nein")
                    multiplikator = WettLogic.CurWette.getOddsNo;
                var winAmount = Convert.ToUInt64(gewinner.Amount * multiplikator);
                user.tokenBalance += winAmount;
                var dcUser = ctx.Channel.Users.FirstOrDefault(x => x.Id == gewinner.UserId);
                outTxt += dcUser.Mention + " hat " + winAmount + " Token gewonnen. Neue Token Balance: " + user.tokenBalance + "\n";
            }
            await ctx.Channel.SendMessageAsync(outTxt).ConfigureAwait(false);
            WettLogic.WriteFile();
        }

        [Command("bet")]
        [Description("Biete auf eine laufende Wette")]
        public async Task Bet(CommandContext ctx, ulong amount, string janein)
        {
            WettLogic ??= new WettenDassLogic();
            WettLogic.TokenGiveOut();

            janein = janein.ToLower();

            if (!WettLogic.CurWette.wetteActive)
            {
                await ctx.Channel.SendMessageAsync("Es läuft derzeit keine Wette.").ConfigureAwait(false);
                return;
            }
            if (WettLogic.CurWette.UserIdStartedBet == ctx.Member.Id)
            {
                await ctx.Channel.SendMessageAsync("Als Eröffner der Wette darfst du nicht teilnehmen.").ConfigureAwait(false);
                return;
            }

            if (janein != "ja" && janein != "nein")
            {
                await ctx.Channel.SendMessageAsync("Ungültige Wette. Bitte benutze \"ja\" oder \"nein\"").ConfigureAwait(false);
                return;
            }

            var bothPots = WettLogic.CurWette.yesPot > 0 && WettLogic.CurWette.noPot > 0;

            var diff = (DateTime.Now - WettLogic.CurWette.BetStarted).Minutes;
            if (diff >= 5 && bothPots)
            {
                await ctx.Channel.SendMessageAsync("Die Zeit um Wetten zu platzieren ist vorrüber.").ConfigureAwait(false);
                return;
            }
            string outTxt = string.Empty;
            var betted = await WettLogic.AddUserToBet(ctx.User.Id, janein, amount, ctx).ConfigureAwait(false);
            if (!betted)
                return;
            if (!bothPots)
            {
                if (WettLogic.CurWette.yesPot > 0 && WettLogic.CurWette.noPot > 0)
                {
                    await ctx.Channel.SendMessageAsync("In beide Töpfe wurde gesetzt - die Wettannahme schließt in 5 Minuten").ConfigureAwait(false);
                    WettLogic.CurWette.BetStarted = DateTime.Now;
                }
            }
            bothPots = WettLogic.CurWette.yesPot > 0 && WettLogic.CurWette.noPot > 0;

            var totalAmount = WettLogic.CurWette.WettEinsaetze.First(x => x.UserId == ctx.Member.Id).Amount;
            outTxt += ctx.User.Mention + " hat " + totalAmount + " Token  auf \"" + janein + "\" gesetzt.\n";
            outTxt += "Im Pot sind " + WettLogic.CurWette.totalPot + " Token\n";
            outTxt += "Ja-Wetten: " + WettLogic.CurWette.yesPot + " Token (Quote: " + WettLogic.CurWette.getOddsYes + ")\n";
            outTxt += "Nein-Wetten: " + WettLogic.CurWette.noPot + " Token (Quote: " + WettLogic.CurWette.getOddsNo + ")\n";
            outTxt += "Wettstarter-Bonus: " + WettLogic.CurWette.DealerCut + " Token\n";
            var user = WettLogic.GetUserFromDb(ctx.User.Id);
            outTxt += ctx.User.Mention + " hat noch " + user.tokenBalance + " Token zum Wetten\n";
            await ctx.Channel.SendMessageAsync(outTxt).ConfigureAwait(false);
        }

        [Command("betInfo")]
        [Description("Zeigt die Laufende Wette")]
        public async Task Bet(CommandContext ctx)
        {
            WettLogic ??= new WettenDassLogic();
            WettLogic.TokenGiveOut();

            if (!WettLogic.CurWette.wetteActive)
            {
                await ctx.Channel.SendMessageAsync("Es läuft derzeit keine Wette.").ConfigureAwait(false);
                return;
            }
            string outTxt = string.Empty;

            outTxt += "Die Wette lautet " + WettLogic.CurWette.curWettTopic + "\n";
            var user = ctx.Channel.Users.FirstOrDefault(x => x.Id == WettLogic.CurWette.UserIdStartedBet);
            outTxt += "Gestartet von " + user.Mention + "\n";
            outTxt += "Im Pot sind " + WettLogic.CurWette.totalPot + " Token\n";
            outTxt += "Ja-Wetten: " + WettLogic.CurWette.yesPot + " Token (Quote: " + WettLogic.CurWette.getOddsYes + ")\n";
            outTxt += "Nein-Wetten: " + WettLogic.CurWette.noPot + " Token (Quote: " + WettLogic.CurWette.getOddsNo + ")\n";
            outTxt += "Wettstarter-Bonus: " + WettLogic.CurWette.DealerCut + " Token\n";

            await ctx.Channel.SendMessageAsync(outTxt).ConfigureAwait(false);
        }

        [Command("token")]
        [Description("Zeigt die verfügbaren Token eines Nutzers an")]
        public async Task Tokens(CommandContext ctx, DiscordUser target)
        {
            WettLogic ??= new WettenDassLogic();
            WettLogic.TokenGiveOut();
            var user = WettLogic.GetUserFromDb(target.Id);
            await ctx.Channel.SendMessageAsync(target.Mention + " hat noch " + user.tokenBalance + " Token zum Wetten").ConfigureAwait(false);
        }

        [Command("token")]
        [Description("Zeigt deine aktuell verfügbaren Token an")]
        public async Task Tokens(CommandContext ctx)
        {
            WettLogic ??= new WettenDassLogic();
            WettLogic.TokenGiveOut();
            var user = WettLogic.GetUserFromDb(ctx.User.Id);
            await ctx.Channel.SendMessageAsync(ctx.User.Mention + " hat noch " + user.tokenBalance + " Token zum Wetten").ConfigureAwait(false);
        }

        [Command("addToken")]
        [RequireOwner]
        [Description("Cheat")]
        public async Task AddTokens(CommandContext ctx, DiscordUser target, ulong amount)
        {
            WettLogic ??= new WettenDassLogic();
            WettLogic.TokenGiveOut();
            var user = WettLogic.GetUserFromDb(target.Id);
            user.tokenBalance += amount;
            user.lastReceived = DateTime.Now;
            WettLogic.WriteFile();
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + target.Mention + " neue Token Balance: " + user.tokenBalance).ConfigureAwait(false);
        }

        [Command("top10")]
        [Description("Eat the rich!")]
        public async Task TopTen(CommandContext ctx)
        {
            WettLogic ??= new WettenDassLogic();
            WettLogic.TokenGiveOut();
            var list = WettLogic.DbData.OrderByDescending(x => x.tokenBalance).ToList();
            int i = 0;
            string outTxt = string.Empty;
            foreach (var item in list)
            {
                i++;
                var dcUser = ctx.Channel.Users.FirstOrDefault(x => x.Id == item.id);
                if (item.id == 414546340777951243)
                {
                    outTxt += i + ". Ratz (" + item.tokenBalance + ")\n";
                }
                else
                {
                    outTxt += i + ". " + dcUser.Mention + " (" + item.tokenBalance + ")\n";
                }

                if (i == 10)
                    break;
            }

            await ctx.Channel.SendMessageAsync(outTxt).ConfigureAwait(false);
        }
    }
}
