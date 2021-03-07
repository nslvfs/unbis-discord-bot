using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
            var fileName = Bot.configJson.badwords;

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
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": o7").ConfigureAwait(false);
        }

        [Command("validate")]
        [Aliases("vd")]
        [RequireOwner]
        [Description("validiert")]
        public async Task ValidateBadwords(CommandContext ctx, [RemainingText] string qry)
        {
            Bot.doCheckBadWords = false;
            var fileName = Bot.configJson.badwords;
            var badWords = new List<string>();
            if (File.Exists(fileName))
            {
                foreach (var line in File.ReadLines(fileName))
                {
                    badWords.Add(line);
                }
            }
            else
            {
                File.Create(fileName).Dispose();
            }
            foreach (var item in badWords)
            {
                if (ctx.Message.Content.Contains(item))
                {
                    await ctx.Channel.SendMessageAsync(item).ConfigureAwait(false);
                    return;
                }
                if (ctx.Message.Content.ToLower().Contains(item.ToLower()))
                {
                    await ctx.Channel.SendMessageAsync(item).ConfigureAwait(false);
                    return;
                }
                var msg = Regex.Replace(ctx.Message.Content, @"([^\w]|_)", "");
                if (msg.Contains(item))
                {
                    await ctx.Channel.SendMessageAsync(item).ConfigureAwait(false);
                    return;
                }
            }
            await ctx.Channel.SendMessageAsync("Alles Ok!").ConfigureAwait(false);
        }

        [Command("bwmode")]
        [Aliases("bwm")]
        [RequireOwner]
        [Description("Badword-Filter an aus")]
        public async Task Badwordmode(CommandContext ctx, [RemainingText] string qry)
        {
            if (ctx.Guild.Id != Bot.guildIdUnbi)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Befehl auf diesen Server unzulässig").ConfigureAwait(false);
                return;
            }
            Bot.doCheckBadWords = !Bot.doCheckBadWords;
            await ctx.Channel.SendMessageAsync("o7").ConfigureAwait(false);
        }
    }
}
