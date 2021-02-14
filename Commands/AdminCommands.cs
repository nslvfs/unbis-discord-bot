using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

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
                foreach(var role in ctx.Member.Roles)
                {
                    await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Role: " + role.Name + " Id: " + role.Id).ConfigureAwait(false);
                }
            }
        }

        [Command("active")]
        [Description("Wer war in den 10 Minuten im Chat aktiv?")]
        public async Task ActiveUser(CommandContext ctx)
        {
            var userList = Shared.GetActiveUsersAsync(ctx);
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
            try { 
                await target.GrantRoleAsync(roleMuted);
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + target.Mention + " jetzt gemuted").ConfigureAwait(false);
            } catch (Exception e)
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
    }
}
