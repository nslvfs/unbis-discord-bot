﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class AdminCommands : BaseCommandModule
    {
        [Command("getId")]
        [RequireOwner]
        [Description("Anzeige der ID von einem Discord-User")]
        public async Task GetId(CommandContext ctx, DiscordUser target)
        {
            if (ctx.Member.Id == 807641560006000670 || ctx.Member.Id == 134719067016658945)
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": UserID: " + target.Id).ConfigureAwait(false);
        }

        [Command("getId")]
        [RequireOwner]
        public async Task GetId(CommandContext ctx)
        {
            if (ctx.Member.Id == 807641560006000670 || ctx.Member.Id == 134719067016658945)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": GuildId: " + ctx.Guild.Id).ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": ChannelId: " + ctx.Channel.Id).ConfigureAwait(false);
            }
        }

        [Command("getRoles")]
        [Description("Anzeige der ID von allen Rollen")]
        [RequireOwner]
        public async Task getRoles(CommandContext ctx)
        {
            if (ctx.Member.Id == 807641560006000670 || ctx.Member.Id == 134719067016658945)
            {
                foreach (var role in ctx.Member.Roles)
                {
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Role: " + role.Name + " Id: " + role.Id).ConfigureAwait(false);
                }
            }
        }

        [Command("active")]
        [Description("Wer war in den 10 Minuten im Chat aktiv?")]
        public async Task ActiveUser(CommandContext ctx)
        {
            var userList = Shared.GetActiveUsers(ctx);
            string result = "Aktive Nutzer: ";
            foreach (var user in userList)
            {
                var name = ((DiscordMember)user).DisplayName;
                result = result + name + ", ";
            }
            result = result.Substring(0, result.Length - 2);
            await ctx.Channel.SendMessageAsync(result).ConfigureAwait(false);
        }

        [Command("mute")]
        [Description("10 Minuten direktes Mute")]
        [RequireOwner]
        public async Task Mute(CommandContext ctx, DiscordMember target)
        {
            if (ctx.Guild.Id != 791393115097137171)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Befehl auf diesen Server unzulässig").ConfigureAwait(false);
                return;
            }
            var roleMuted = ctx.Guild.GetRole(807921762570469386);
            try
            {
                await target.GrantRoleAsync(roleMuted);
                Bot.RemoveUserfromMessageArchiv(target.Id);
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + target.Mention + " jetzt gemuted").ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + e.Message).ConfigureAwait(false);
            }
            Thread.Sleep(1000 * 10 * 60); // 10 minuten
            await target.RevokeRoleAsync(roleMuted);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + target.Mention + " jetzt nicht mehr gemuted").ConfigureAwait(false);
        }

        [Command("kick")]
        [Description("kickt ein Mitglied")]
        [RequireOwner]
        public async Task kick(CommandContext ctx, DiscordMember target)
        {
            if (ctx.Guild.Id != 791393115097137171)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Befehl auf diesen Server unzulässig").ConfigureAwait(false);
                return;
            }
            await target.RemoveAsync("!kick von " + ctx.Member.DisplayName);
            await ctx.Channel.SendMessageAsync("|▀▄▀▄▀| unbequem ihm sein discord sagt danke |▀▄▀▄▀| ♫♪♫ Porsche Sportauspuff Sound ♫♪♫").ConfigureAwait(false);
        }

        [Command("Stille")]
        [Description("Stille herrscht")]
        [RequireOwner]
        public async Task Stille(CommandContext ctx)
        {
            if (ctx.Guild.Id != 791393115097137171)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Befehl auf diesen Server unzulässig").ConfigureAwait(false);
                return;
            }
            Bot.silentMode = !Bot.silentMode;
            await ctx.Channel.SendMessageAsync("o7").ConfigureAwait(false);
        }

        [Command("s10")]
        [Description("Sagt etwas 10x")]
        [RequireOwner]
        public async Task SayTenTimes(CommandContext ctx, params string[] args)
        {
            string result = string.Empty;
            foreach (var text in args)
            {
                result = result + " " + text;
            }
            for (var i = 0; i <= 9; i++)
            {
                await ctx.Channel.SendMessageAsync(result).ConfigureAwait(false);
            }
        }

        [Command("sudo"), Description("sudo"), Hidden, RequireOwner]
        public async Task Sudo(CommandContext ctx, [Description("Ziel")] DiscordMember member, [RemainingText, Description("Befehl")] string command)
        {
            await ctx.TriggerTypingAsync();
            var cmds = ctx.CommandsNext;
            var cmd = cmds.FindCommand(command, out var customArgs);
            var fakeContext = cmds.CreateFakeContext(member, ctx.Channel, command, ctx.Prefix, cmd, customArgs);
            await cmds.ExecuteCommandAsync(fakeContext);
        }

        [Command("nick"), Description("Nicknamen von dritten ändern"), RequirePermissions(Permissions.ManageNicknames)]
        public async Task ChangeNickname(CommandContext ctx, [Description("Ziel")] DiscordMember member, [RemainingText, Description("Neuer Nick")] string new_nickname)
        {
            try
            {
                await member.ModifyAsync(x =>
                {
                    x.Nickname = new_nickname;
                    x.AuditLogReason = $"Changed by {ctx.User.Username} ({ctx.User.Id}).";
                });
                var emoji = DiscordEmoji.FromName(ctx.Client, "o7");
                await ctx.RespondAsync(emoji);
            }
            catch (Exception)
            {
                var emoji = DiscordEmoji.FromName(ctx.Client, ":c");
                await ctx.RespondAsync(emoji);
            }
        }
    }
}

