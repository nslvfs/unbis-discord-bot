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
            if (WettLogic.CurWette.UserIdStartedBet != ctx.Member.Id)
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
            var gewinners = WettLogic.CurWette.WettEinsaetze.Where(x => x.Vote == result).ToList();
            foreach (var gewinner in gewinners)
            {
                var user = WettLogic.DbData.FirstOrDefault(x => x.id == gewinner.UserId);
                var multiplikator = WettLogic.CurWette.getOddsYes;
                if (result == "nein")
                    multiplikator = WettLogic.CurWette.getOddsNo;
                var winAmount = Convert.ToUInt64(gewinner.Amount * multiplikator);
                user.tokenBalance += winAmount;
                var dcUser = ctx.Channel.Users.FirstOrDefault(x => x.Id == gewinner.UserId);
                await ctx.Channel.SendMessageAsync(dcUser.Mention + " hat " + winAmount + " Token gewonnen. Neue Token Balance: " + user.tokenBalance).ConfigureAwait(false);
            }
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

            var diff = (DateTime.Now - WettLogic.CurWette.BetStarted).Minutes;
            if (diff >= 5)
            {
                await ctx.Channel.SendMessageAsync("Die Zeit um Wetten zu platzieren ist vorrüber.").ConfigureAwait(false);
                return;
            }

            var betted = await WettLogic.AddUserToBet(ctx.User.Id, janein, amount, ctx).ConfigureAwait(false);
            if (!betted)
                return;
            var totalAmount = WettLogic.CurWette.WettEinsaetze.First(x => x.UserId == ctx.Member.Id).Amount;
            await ctx.Channel.SendMessageAsync(ctx.User.Mention + " hat " + totalAmount + " Token  auf \"" + janein + "\" gesetzt.").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("Im Pot sind " + WettLogic.CurWette.totalPot + " Token").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("Ja-Wetten " + WettLogic.CurWette.yesPot + " Token (Quote: " + WettLogic.CurWette.getOddsYes + ")").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("Nein-Wetten " + WettLogic.CurWette.noPot + " Token (Quote: " + WettLogic.CurWette.getOddsNo + ")").ConfigureAwait(false);
            var user = WettLogic.GetUserFromDb(ctx.User.Id);
            await ctx.Channel.SendMessageAsync(ctx.User.Mention + " hat noch " + user.tokenBalance + " Token zum Wetten").ConfigureAwait(false);
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
            await ctx.Channel.SendMessageAsync("Die Wette lautet " + WettLogic.CurWette.curWettTopic).ConfigureAwait(false);
            var user = ctx.Channel.Users.FirstOrDefault(x => x.Id == WettLogic.CurWette.UserIdStartedBet);
            await ctx.Channel.SendMessageAsync("Gestartet von " + user.Mention).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("Im Pot sind " + WettLogic.CurWette.totalPot + " Token").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("Ja-Wetten " + WettLogic.CurWette.yesPot + " Token (Quote: " + WettLogic.CurWette.getOddsYes + ")").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("Nein-Wetten " + WettLogic.CurWette.noPot + " Token (Quote: " + WettLogic.CurWette.getOddsNo + ")").ConfigureAwait(false);
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
                outTxt += i + ". " + dcUser.Mention + " (" + item.tokenBalance + ")\n";
                
                if (i == 10)
                    break;
            }

            await ctx.Channel.SendMessageAsync(outTxt).ConfigureAwait(false);
        }
    }
}
