using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace unbis_discord_bot.Logic
{
    public class MessageCacheCleanupService : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    Bot.ArchivMessages.RemoveAll(item => item.Timestamp.AddMinutes(10) < DateTimeOffset.Now);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
