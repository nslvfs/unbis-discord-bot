using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.IO;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    class BadWords : BaseCommandModule
    {
        [Command("addbadwort")]
        [Aliases("abw")]
        [RequireOwner]
        public async Task AddBadWord(CommandContext ctx, params string[] args)
        {
            var fileName = Bot.configJson.badwords; ;

            if (!File.Exists(fileName))
            {
                File.Create(fileName).Dispose();
            }

            using (StreamWriter w = File.AppendText(fileName))
            {
                foreach (var newline in args)
                {
                    w.WriteLine(newline.Trim());
                }
            }
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": o7");
        }
    }
}
