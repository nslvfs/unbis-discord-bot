using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace unbis_discord_bot.Logic
{
    public class BotHostedService : BackgroundService
    {
        private readonly Bot _bot = new();

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return _bot.RunAsync(stoppingToken);
        }
    }
}
