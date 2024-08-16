using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using unbis_discord_bot.Logic;

namespace unbis_discord_bot.Commands
{
    public class WettCommands : BaseCommandModule
    {
        public WettenDassLogic WettLogic { get; set; }
        [Command("newbet")]
        [Description("Startet eine neue Wette")]
        public async Task StartNewBet(CommandContext ctx, params string[] args)
        {
            WettLogic ??= new WettenDassLogic();
        }
    }
}
