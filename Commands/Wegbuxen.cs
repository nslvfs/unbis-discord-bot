using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using DSharpPlus.Interactivity.Enums;
using System.Linq;
using System.Text;
using DSharpPlus.Interactivity.Extensions;
using System.Threading;

namespace unbis_discord_bot.Commands
{
    public class Wegbuxen : BaseCommandModule
    {
        [Command("gegen")]
        [Description("Startet einen Poll ob ein User auf die Stille Treppe soll. Funktioniert nur auf unbequem ihm sein Discord")]
        public async Task Gegen(CommandContext ctx, DiscordUser target)
        {
            if (ctx.Guild.Id != 791393115097137171)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Befehl auf diesen Server unzulässig").ConfigureAwait(false);
                return;
            }

            TimeSpan duration = new TimeSpan(0, 0, 2, 0, 0);
            var client = ctx.Client;
            var interactivity = client.GetInteractivity();

            if (target.Id == 807641560006000670 || target.Id == 134719067016658945)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Unzulässiges Ziel " + target.Mention).ConfigureAwait(false);
                return;
            }

            var userList = Shared.GetActiveUsers(ctx);
            bool validTarget = false;

            foreach (var user in userList)
            {
                if (user.Id == target.Id)
                {
                    validTarget = true;
                    break;
                }
            }

            if (!validTarget)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Unzulässiges Ziel " + target.Mention).ConfigureAwait(false);
                return;
            }

            var minYes = userList.Count / 2;
            var _pollEmojiCache = new[] {
                        DiscordEmoji.FromName(client, ":white_check_mark:"),
                        DiscordEmoji.FromName(client, ":x:")
                    };

            var pollStartText = new StringBuilder();
            pollStartText.Append(target.Mention + "wegbuxen? (" + minYes + " Stimme(n) benötigt)");
            var pollStartMessage = await ctx.RespondAsync(pollStartText.ToString());

            var pollResult = await interactivity.DoPollAsync(pollStartMessage, _pollEmojiCache, PollBehaviour.DeleteEmojis, duration);

            var yesVotes = 0;
            var noVotes = 0;
            if (pollResult[0].Emoji == ":white_check_mark:")
            {
                yesVotes = pollResult[0].Total;
                noVotes = pollResult[1].Total;
            }
            else
            {
                yesVotes = pollResult[1].Total;
                noVotes = pollResult[0].Total;
            }
            var pollResultText = new StringBuilder();
            pollResultText.AppendLine(target.Mention + "wegbuxen? (" + minYes + " Stimme(n) benötigt)");
            pollResultText.Append("Ergebnis: Dafür: " + yesVotes + " Dagegen: " + noVotes + "\n");
            pollResultText.Append("**");
            if (yesVotes > noVotes)
            {
                if (yesVotes >= minYes)
                {
                    pollResultText.Append("Buxung erfolgreich");
                    await ctx.RespondAsync(pollResultText.ToString());
                    var roleMuted = ctx.Guild.GetRole(807921762570469386);
                    var userId = ctx.Message.MentionedUsers.First().Id;
                    DiscordMember member = await ctx.Guild.GetMemberAsync(userId);
                    //await member.RemoveAsync("Im Namen des Volkes.");                  
                    await ctx.Channel.SendMessageAsync("|▀▄▀▄▀| unbequem ihm sein discord sagt danke |▀▄▀▄▀| ♫♪♫ Porsche Sportauspuff Sound ♫♪♫").ConfigureAwait(false);
                    await member.GrantRoleAsync(roleMuted);
                    Thread.Sleep(1000 * 60 * 10); // 10 Min
                    await member.RevokeRoleAsync(roleMuted);
                    await ctx.Channel.SendMessageAsync(member.Mention + " jetzt nicht mehr gemuted").ConfigureAwait(false);
                }
                else
                {
                    pollResultText.Append("Votekick gescheitert (kritische Masse nicht erreicht)");
                    await ctx.RespondAsync(pollResultText.ToString());
                }
            }
            else if (yesVotes == noVotes)
            {
                pollResultText.Append("Kann man nichts machen");
                await ctx.RespondAsync(pollResultText.ToString());
            }
            else
            {
                pollResultText.Append("Votekick gescheitert");
                await ctx.RespondAsync(pollResultText.ToString());
            }
        }
    }
}
