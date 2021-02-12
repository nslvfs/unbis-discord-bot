using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace unbis_discord_bot.Commands
{
    public class GetIds : BaseCommandModule
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
    }
}
