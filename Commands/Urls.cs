using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class Urls : BaseCommandModule
    {
        [Command("chefkoch")]
        [Description("Zufälliges Rezept von chefkoch")]
        public async Task GetRandomRezept(CommandContext ctx)
        {
            var newUrl = await Shared.GetRedirect("https://www.chefkoch.de/rezepte/zufallsrezept/").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + newUrl).ConfigureAwait(false);
        }

        [Command("rmv")]
        [Description("Zufälliges Video von motherless")]
        public async Task GetRandomPornVideo(CommandContext ctx)
        {
            var newUrl = await Shared.GetRedirect("https://motherless.com/random/video").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + newUrl).ConfigureAwait(false);
        }

        [Command("rmb")]
        [Description("Zufälliges Bild von motherless")]
        public async Task GetRandomPornPic(CommandContext ctx)
        {
            var newUrl = await Shared.GetRedirect("https://motherless.com/random/image").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + newUrl).ConfigureAwait(false);
        }
    }
}
