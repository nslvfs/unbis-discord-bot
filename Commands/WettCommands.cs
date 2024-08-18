using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
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
                await ctx.Channel.SendMessageAsync("Es l�uft bereits eine Wette: \"" + WettLogic.CurWette.curWettTopic + "\"").ConfigureAwait(false);
                return;
            }

            var wettTopic = string.Join(" ", args);
            if (Bot.CheckBadWords(WettLogic.CurWette.curWettTopic, true))
            {
                await ctx.Channel.SendMessageAsync("Ung�ltige Wette.").ConfigureAwait(false);
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
            if (!WettLogic.CurWette.wetteActive)
            {
                await ctx.Channel.SendMessageAsync("Es l�uft derzeit keine Wette.").ConfigureAwait(false);
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
                await ctx.Channel.SendMessageAsync("Kein G�ltiges Ergebnis. Ergebis kann nur Ja oder Nein sein").ConfigureAwait(false);
                return;
            }

            WettLogic.CurWette.wetteActive = false;
            await ctx.Channel.SendMessageAsync("Die Wette \"" + WettLogic.CurWette.curWettTopic + "\" wurde mit dem Ergebnis \"" + result + "\" beendet.").ConfigureAwait(false);
            // TODO Cashout
        }

        [Command("bet")]
        [Description("Biete auf eine laufende Wette")]
        public async Task Bet(CommandContext ctx, ulong amount, string janein)
        {
            janein = janein.ToLower();

            if (!WettLogic.CurWette.wetteActive)
            {
                await ctx.Channel.SendMessageAsync("Es l�uft derzeit keine Wette.").ConfigureAwait(false);
                return;
            }
            if (WettLogic.CurWette.UserIdStartedBet == ctx.Member.Id)
            {
                await ctx.Channel.SendMessageAsync("Als Er�ffner der Wette darfst du nicht teilnehmen.").ConfigureAwait(false);
                return;
            }

            if(janein != "ja" && janein != "nein")
            {
                await ctx.Channel.SendMessageAsync("Ung�ltige Wette. Bitte benutze \"ja\" oder \"nein\"").ConfigureAwait(false);
                return;
            }

            var diff = (DateTime.Now - WettLogic.CurWette.BetStarted).Minutes;
            if(diff >= 5)
            {
                await ctx.Channel.SendMessageAsync("Die Zeit um Wetten zu platzieren ist vorr�ber.").ConfigureAwait(false);
                return;
            }


            
            var betted = await WettLogic.AddUserToBet(ctx.User.Id, janein, amount, ctx).ConfigureAwait(false);
            if (!betted)
                return;
            var totalAmount = WettLogic.CurWette.WettEinsaetze.First(x => x.UserId == ctx.Member.Id).Amount;
            await ctx.Channel.SendMessageAsync(ctx.User.Mention + " hat " + totalAmount + "  auf \"" + janein + "\" gesetzt.").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("Im Pot sind " + WettLogic.CurWette.totalPot).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("Ja-Wetten " + WettLogic.CurWette.yesPot + " (Quote: "+ WettLogic.CurWette.getOddsYes + ")").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("Nein-Wetten " + WettLogic.CurWette.noPot + " (Quote: " + WettLogic.CurWette.getOddsNo+ ")").ConfigureAwait(false);
        }

        [Command("betInfo")]
        [Description("Zeigt die Laufende Wette")]
        public async Task Bet(CommandContext ctx)
        {
            if (!WettLogic.CurWette.wetteActive)
            {
                await ctx.Channel.SendMessageAsync("Es l�uft derzeit keine Wette.").ConfigureAwait(false);
                return;
            }
            await ctx.Channel.SendMessageAsync("Die Wette lautet " + WettLogic.CurWette.curWettTopic).ConfigureAwait(false);
            var user = ctx.Channel.Users.FirstOrDefault(x => x.Id == WettLogic.CurWette.UserIdStartedBet);
            await ctx.Channel.SendMessageAsync("Gestartet von " + user.Mention).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("Im Pot sind " + WettLogic.CurWette.totalPot).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("Ja-Wetten " + WettLogic.CurWette.yesPot + " (Quote: " + WettLogic.CurWette.getOddsYes + ")").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("Nein-Wetten " + WettLogic.CurWette.noPot + " (Quote: " + WettLogic.CurWette.getOddsNo + ")").ConfigureAwait(false);
        }

        [Command("addTokens")]
        [RequireOwner]
        [Description("Cheat")]
        public async Task AddTokens(CommandContext ctx, DiscordUser target, ulong amount)
        {
            WettLogic ??= new WettenDassLogic();
            var user = WettLogic.GetUserFromDb(target.Id);
            user.tokenBalance += amount;
            user.lastReceived = DateTime.Now;
            WettLogic.WriteFile();
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + target.Mention + " neue Token Balance: " + user.tokenBalance).ConfigureAwait(false);
        }

    }
}
