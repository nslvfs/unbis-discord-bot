using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
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
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": UserID: " + target.Id).ConfigureAwait(false);
        }

        [Command("getId")]
        [RequireOwner]
        public async Task GetId(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": GuildId: " + ctx.Guild.Id).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": ChannelId: " + ctx.Channel.Id).ConfigureAwait(false);
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
            if (ctx.Guild.Id != Bot.guildIdUnbi)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Befehl auf diesen Server unzulässig").ConfigureAwait(false);
                return;
            }
            await Bot.Mute(ctx.Channel, target, ctx.Guild).ConfigureAwait(false);
        }

        [Command("kick")]
        [Description("kickt ein Mitglied")]
        [RequireOwner]
        public async Task kick(CommandContext ctx, DiscordMember target)
        {
            if (ctx.Guild.Id != Bot.guildIdUnbi)
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
            if (ctx.Guild.Id != Bot.guildIdUnbi)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Befehl auf diesen Server unzulässig").ConfigureAwait(false);
                return;
            }
            Bot.silentMode = !Bot.silentMode;
            await ctx.Channel.SendMessageAsync("o7").ConfigureAwait(false);
        }

        [Command("RandomMode")]
        [Description("Zufall herrscht")]
        [RequireOwner]
        public async Task RandomMode(CommandContext ctx)
        {
            if (ctx.Guild.Id != Bot.guildIdUnbi)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Befehl auf diesen Server unzulässig").ConfigureAwait(false);
                return;
            }
            Bot.randomMode = !Bot.randomMode;
            await ctx.Channel.SendMessageAsync("o7").ConfigureAwait(false);
        }

        [Command("CryptoMode")]
        [Description("Verschlüßelung herrscht")]
        [RequireOwner]
        public async Task CryptoMode(CommandContext ctx)
        {
            if (ctx.Guild.Id != Bot.guildIdUnbi)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Befehl auf diesen Server unzulässig").ConfigureAwait(false);
                return;
            }
            Bot.cryptoMode = !Bot.cryptoMode;
            await ctx.Channel.SendMessageAsync("o7").ConfigureAwait(false);
        }



        [Command("sudo"), Description("sudo"), Hidden, RequireOwner]
        public async Task Sudo(CommandContext ctx, [Description("Ziel")] DiscordMember member, [RemainingText, Description("Befehl")] string command)
        {
            var cmds = ctx.CommandsNext;
            var cmd = cmds.FindCommand(command, out var customArgs);
            var fakeContext = cmds.CreateFakeContext(member, ctx.Channel, command, ctx.Prefix, cmd, customArgs);
            await cmds.ExecuteCommandAsync(fakeContext).ConfigureAwait(false);
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
                await ctx.Channel.SendMessageAsync("o7").ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                await ctx.Channel.SendMessageAsync(":C").ConfigureAwait(false);
            }
        }

    }
}

