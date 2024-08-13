using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System.IO;
using System.Text;
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
            result = result[..^2];
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
        public async Task Kick(CommandContext ctx, DiscordMember target)
        {
            if (ctx.Guild.Id != Bot.guildIdUnbi)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": Befehl auf diesen Server unzulässig").ConfigureAwait(false);
                return;
            }
            await target.RemoveAsync("!kick von " + ctx.Member.DisplayName);
            await ctx.Channel.SendMessageAsync("|▀▄▀▄▀| unbequem ihm sein discord sagt danke |▀▄▀▄▀| ♫♪♫ Porsche Sportauspuff Sound ♫♪♫").ConfigureAwait(false);
        }

        [Command("rehash")]
        [Description("config Datei neu einlesen")]
        [RequireOwner]
        public async Task Rehash(CommandContext ctx)
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            Bot.ConfigJson = JsonConvert.DeserializeObject<ConfigJson>(json);
            await ctx.Channel.SendMessageAsync(Bot.ConfigJson.positivAnswer).ConfigureAwait(false);
        }

    }
}

