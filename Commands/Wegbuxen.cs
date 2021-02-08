using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using System.Linq;
using System.Text;
using DSharpPlus;
using DSharpPlus.Interactivity.Extensions;
using System.Collections.Generic;

namespace unbis_discord_bot.Commands
{
    public class Wegbuxen : BaseCommandModule
    {
        [Command("gegen")]
        [Description("Startet einen Poll ob ein User auf die Stille Treppe soll. Funktioniert nur auf unbequem ihm sein Discord")]
        public async Task Gegen(CommandContext ctx, DiscordUser target)
        {
            TimeSpan duration = new TimeSpan(0, 0, 2, 0, 0);

            var roles = ctx.Guild.Roles;
            var client = ctx.Client;

            var interactivity = client.GetInteractivity();

            if (target.Id == 807641560006000670 || target.Id == 134719067016658945)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Unzulässiges Ziel " + target.Mention).ConfigureAwait(false);
                return;
            }

            List<string> userList = new List<string>();

            foreach (var user in ctx.Channel.Users)
            {
                if (!user.IsBot)
                {
                    userList.Add(user.DisplayName);
                }
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
            var yesVotes = pollResult[1].Total;
            var noVotes = pollResult[0].Total;
            var pollResultText = new StringBuilder();
            pollResultText.AppendLine(target.Mention + "wegbuxen? (" + minYes + " Stimme(n) benötigt)");
            pollResultText.Append("Ergebnis: ");
            pollResultText.Append("**");
            if (yesVotes > noVotes)
            {
                if(yesVotes >= minYes) { 
                    pollResultText.Append("Buxung erfolgreich");
                    await ctx.RespondAsync(pollResultText.ToString());
                    var roleMuted = ctx.Guild.GetRole(807921762570469386);
                    var userId = ctx.Message.MentionedUsers.First().Id;
                    DiscordMember member = await ctx.Guild.GetMemberAsync(userId);
                    await member.GrantRoleAsync(roleMuted);
                    Thread.Sleep(1000 * 60 * 10); // 10 Min
                    await member.RevokeRoleAsync(roleMuted);
                } else
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

        [Command("mute")]
        [Description("10 Minuten direktes Mute")]
        [RequireOwner]
        public async Task Mute(CommandContext ctx, DiscordMember target)
        {
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + target.Mention + " jetzt gemuted").ConfigureAwait(false);
            var roleMuted = ctx.Guild.GetRole(807921762570469386);
            await target.GrantRoleAsync(roleMuted);
            Thread.Sleep(1000 * 10 * 60); // 10 minuten
            await target.RevokeRoleAsync(roleMuted);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + target.Mention + " jetzt nicht mehr gemuted").ConfigureAwait(false);
        }
    }
}
