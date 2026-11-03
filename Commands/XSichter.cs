using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SkiaSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace unbis_discord_bot.Commands
{
    public class XSichter : BaseCommandModule
    {
        [Command("xsichter")]
        [Aliases("xs")]
        [Description("Random xSicht")]
        public async Task RndXSichter(CommandContext ctx)
        {
            await SendRandomXSicht(ctx, Bot.ConfigJson.positivAnswer).ConfigureAwait(false);
        }

        [Command("dxsichter")]
        [Aliases("dxs")]
        [Description("Random distorted xSicht")]
        public async Task RndDXSichter(CommandContext ctx)
        {
            if (ctx.Channel.Id != Bot.ConfigJson.ChannelIdRotz)
            {
                await ctx.Channel.SendMessageAsync(ctx.Member.Mention + ": " + Bot.ConfigJson.negativAnswer).ConfigureAwait(false);
                return;
            }
            await SendRandomXSicht(ctx, ".distort").ConfigureAwait(false);
        }

        private const int MaxMemeWidth = 500;

        private static async Task SendRandomXSicht(CommandContext ctx, string content)
        {
            var files = Directory.GetFiles(Bot.ConfigJson.xSichterPath, "*.*", SearchOption.AllDirectories);
            var rndIndex = Shared.GenerateRandomNumber(0, files.Length - 1);
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".jpg");
            try
            {
                ResizeImageAndSaveThumb(files[rndIndex], tempFile);
                using var fs = new FileStream(tempFile, FileMode.Open, FileAccess.Read);
                await new DiscordMessageBuilder()
                    .WithContent(content)
                    .AddFile("xsichter.jpg", fs)
                    .SendAsync(ctx.Channel)
                    .ConfigureAwait(false);
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        private static void ResizeImageAndSaveThumb(string inputPath, string outputPath)
        {
            using var input = File.OpenRead(inputPath);
            using var original = SKBitmap.Decode(input);

            var width = original.Width;
            var height = original.Height;
            if (width > MaxMemeWidth)
            {
                height = (int)(original.Height * ((double)MaxMemeWidth / width));
                width = MaxMemeWidth;
            }

            using var resized = original.Resize(new SKImageInfo(width, height), new SKSamplingOptions(SKCubicResampler.Mitchell));
            using var image = SKImage.FromBitmap(resized);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            using var output = File.OpenWrite(outputPath);
            data.SaveTo(output);
        }
    }
}
