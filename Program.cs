using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using unbis_discord_bot.Logic;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSystemd();
builder.Services.AddHostedService<BotHostedService>();
builder.Services.AddHostedService<MessageCacheCleanupService>();

await builder.Build().RunAsync();
