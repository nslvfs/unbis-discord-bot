using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class Esso : BaseCommandModule
    {
        [Command("durstometer")]
        [Description("Durstigkeit in Prozent eines Users")]
        public async Task Durstometer(CommandContext ctx, DiscordUser target)
        {
            int res = 100;
            if (target.Id != Bot.userIdEsso)
            {
                res = Shared.GenerateRandomNumber(1, res);
            }
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + target.Mention + " ist " + res + "% durstig").ConfigureAwait(false);
        }

        [Command("normimeter")]
        [Description("Normigkeit in Prozent eines Users")]
        public async Task Normimeter(CommandContext ctx, DiscordUser target)
        {
            int res = 100;
            if (target.Id != Bot.userIdEsso && target.Id != Bot.userIdRattan)
            {
                res = Shared.GenerateRandomNumber(1, 30);
            }
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + target.Mention + " ist " + res + "% Normie").ConfigureAwait(false);
        }
    }
}
