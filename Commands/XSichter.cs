using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
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
            int maxMemeWidth = 500;
            var files = Directory.GetFiles(Bot.configJson.xSichterPath, "*.*", SearchOption.AllDirectories);
            var rndIndex = Shared.GenerateRandomNumber(0, files.Length - 1);
            Image photo = new Bitmap(files[rndIndex]);
            var divisor = photo.Width / maxMemeWidth;
            var newHeight = photo.Height / divisor;

            ResizeImageAndSaveThumb(files[rndIndex], newHeight, maxMemeWidth, ImageFormat.Jpeg);
            using (var fs = new FileStream("temp.jpg", FileMode.Open, FileAccess.Read))
            {
                await new DiscordMessageBuilder()
                    .WithContent("o7")
                    .WithFiles(new System.Collections.Generic.Dictionary<string, Stream>() { { "xsichter.jpg", fs } })
                    .SendAsync(ctx.Channel)
                    .ConfigureAwait(false);
            }
        }


        [Command("dxsichter")]
        [Aliases("dxs")]
        [Description("Random distorted xSicht")]
        public async Task RndDXSichter(CommandContext ctx)
        {
            int maxMemeWidth = 500;
            var files = Directory.GetFiles(Bot.configJson.xSichterPath, "*.*", SearchOption.AllDirectories);
            var rndIndex = Shared.GenerateRandomNumber(0, files.Length - 1);
            Image photo = new Bitmap(files[rndIndex]);
            var divisor = photo.Width / maxMemeWidth;
            var newHeight = photo.Height / divisor;

            ResizeImageAndSaveThumb(files[rndIndex], newHeight, maxMemeWidth, ImageFormat.Jpeg);
            using (var fs = new FileStream("temp.jpg", FileMode.Open, FileAccess.Read))
            {
                await new DiscordMessageBuilder()
                    .WithContent(".distort")
                    .WithFiles(new System.Collections.Generic.Dictionary<string, Stream>() { { "xsichter.jpg", fs } })
                    .SendAsync(ctx.Channel)
                    .ConfigureAwait(false);
            }
        }
        public static void ResizeImageAndSaveThumb(string FileNameInput, double ResizeHeight, double ResizeWidth, ImageFormat OutputFormat)
        {
            using (Image photo = new Bitmap(FileNameInput))
            {
                double aspectRatio = (double)photo.Width / photo.Height;
                double boxRatio = ResizeWidth / ResizeHeight;
                double scaleFactor = 0;

                if (photo.Width < ResizeWidth && photo.Height < ResizeHeight)
                {
                    scaleFactor = 1.0;
                }
                else
                {
                    if (boxRatio > aspectRatio)
                        scaleFactor = ResizeHeight / photo.Height;
                    else
                        scaleFactor = ResizeWidth / photo.Width;
                }

                int newWidth = (int)(photo.Width * scaleFactor);
                int newHeight = (int)(photo.Height * scaleFactor);

                using (Bitmap bmp = new Bitmap(newWidth, newHeight))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        g.DrawImage(photo, 0, 0, newWidth, newHeight);

                        if (ImageFormat.Png.Equals(OutputFormat))
                        {
                            bmp.Save("temp.png", OutputFormat);
                        }
                        else if (ImageFormat.Jpeg.Equals(OutputFormat))
                        {
                            ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
                            EncoderParameters encoderParameters;
                            using (encoderParameters = new EncoderParameters(1))
                            {
                                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                                bmp.Save("temp.jpg", info[1], encoderParameters);
                            }
                        }
                    }
                }
            }
        }
    }
}
