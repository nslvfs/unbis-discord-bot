﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class Wegbuxen : BaseCommandModule
    {
        SemaphoreSlim _semaphoregate = new SemaphoreSlim(1, 1);

        [Command("gegen")]
        [Description("Startet einen Poll ob ein User auf die Stille Treppe soll. Funktioniert nur auf unbequem ihm sein Discord")]
        public async Task Gegen(CommandContext ctx, DiscordUser target)
        {
            var running = false;
            if (_semaphoregate.CurrentCount > 1)
            {
                running = true;
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Es läuft bereits eine Abstimmung!").ConfigureAwait(false);
            }
            await _semaphoregate.WaitAsync();

            if (running)
            {
                _semaphoregate.Release();
                return;
            }
            if (ctx.Guild.Id != Bot.guildIdUnbi)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Befehl auf diesen Server unzulässig").ConfigureAwait(false);
                _semaphoregate.Release();
                return;
            }

            TimeSpan duration = new TimeSpan(0, 0, 2, 0, 0);
            var client = ctx.Client;
            var interactivity = client.GetInteractivity();

            if (target.Id == Bot.botIdSelf || target.Id == Bot.userIdvfs)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Unzulässiges Ziel " + target.Mention).ConfigureAwait(false);
                _semaphoregate.Release();
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
                _semaphoregate.Release();
                return;
            }

            var minYes = userList.Count / 2;
            var _pollEmojiCache = new[] {
                        DiscordEmoji.FromName(client, ":white_check_mark:"),
                        DiscordEmoji.FromName(client, ":x:")
                    };

            var pollStartText = new StringBuilder();
            pollStartText.Append(target.Mention + " wegbuxen? (" + minYes + " Stimme(n) benötigt)");
            var pollStartMessage = await ctx.RespondAsync(pollStartText.ToString());

            var pollResult = await interactivity.DoPollAsync(pollStartMessage, _pollEmojiCache, PollBehaviour.DeleteEmojis, duration);

            int yesVotes;
            int noVotes;
            if (pollResult[0].Emoji == "✅")
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
                    var roleMuted = ctx.Guild.GetRole(Bot.roleIdMuted);
                    var userId = ctx.Message.MentionedUsers.First().Id;
                    DiscordMember member = await ctx.Guild.GetMemberAsync(userId);
                    await ctx.Channel.SendMessageAsync("|▀▄▀▄▀| unbequem ihm sein discord sagt danke |▀▄▀▄▀| ♫♪♫ Porsche Sportauspuff Sound ♫♪♫").ConfigureAwait(false);
                    await member.GrantRoleAsync(roleMuted);
                    _ = Bot.RemoveUserfromMessageArchiv(ctx.Member.Id);
                    Thread.Sleep(1000 * 60 * 10);
                    await member.RevokeRoleAsync(roleMuted);
                    var outChannel = ctx.Guild.GetChannel(Bot.channelIdRotz);
                    await outChannel.SendMessageAsync(member.Mention + " jetzt nicht mehr still").ConfigureAwait(false);
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
            _semaphoregate.Release();
        }
    }
}
