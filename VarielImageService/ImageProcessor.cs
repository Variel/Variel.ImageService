using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;
using Variel.ImageService.Data;

namespace Variel.ImageService
{
    public static class ImageProcessor
    {

        public static async Task<Stream> AutoRotateAsync(Stream sourceStream)
        {
            await using var memStream = new MemoryStream();
            await sourceStream.CopyToAsync(memStream);
            memStream.Seek(0, SeekOrigin.Begin);

            return AutoRotate(memStream);
        }

        public static Stream AutoRotate(Stream sourceStream)
        {
            using var codec = SKCodec.Create(sourceStream);
            using var source = SKBitmap.Decode(codec);

            var origin = codec.EncodedOrigin;

            var (width, height, transform) = GetAutoRotateInfo(source, origin);

            var imageInfo = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(imageInfo);

            using var paint = new SKPaint
            {
                IsAntialias = true,
                FilterQuality = SKFilterQuality.High
            };

            transform(surface.Canvas);

            surface.Canvas.DrawBitmap(source, imageInfo.Rect, paint);
            surface.Canvas.Flush();

            var resultStream = new MemoryStream();

            using var image = surface.Snapshot();
            using var data = image.Encode(codec.EncodedFormat, 80);
            data.SaveTo(resultStream);

            resultStream.Seek(0, SeekOrigin.Begin);
            return resultStream;
        }

        public static async Task<Stream> ResizeAsync(Stream sourceStream, ProcessingPreset settings)
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));

            await using var memStream = new MemoryStream();
            await sourceStream.CopyToAsync(memStream);
            memStream.Seek(0, SeekOrigin.Begin);

            return Resize(memStream, settings);
        }

        public static Stream Resize(Stream sourceStream, ProcessingPreset settings)
        {
            if (settings.Width == null || settings.Height == null)
                throw new InvalidOperationException("Width and Height must be specified");

            var source = SKBitmap.Decode(sourceStream);
            var preserveAlpha = settings.PreserveAlpha ?? source.AlphaType == SKAlphaType.Unpremul;

            var imageInfo = new SKImageInfo(settings.Width.Value, settings.Height.Value);
            using var surface = SKSurface.Create(imageInfo);
            using var paint = new SKPaint
            {
                IsAntialias = true,
                FilterQuality = SKFilterQuality.High
            };

            if (settings.ResizeOption == ResizeOption.StretchToFill)
            {
                surface.Canvas.DrawBitmap(source, SKRect.Create(imageInfo.Width, imageInfo.Height), paint);
            }
            else
            {
                float scale = 1;

                switch (settings.ResizeOption)
                {
                    case ResizeOption.None:
                        break;

                    case ResizeOption.AspectFit:
                        scale = Math.Min((float) imageInfo.Width / source.Width, (float) imageInfo.Height / source.Height);
                        break;

                    case ResizeOption.AspectFill:
                        scale = Math.Max((float) imageInfo.Width / source.Width, (float) imageInfo.Height / source.Height);
                        break;
                }

                SKRect display = CalculateDisplayRect(SKRect.Create(imageInfo.Width, imageInfo.Height),
                    scale * source.Width, scale * source.Height);

                surface.Canvas.DrawBitmap(source, display, paint);
            }

            using var resultImage = surface.Snapshot();
            using var resultData =
                resultImage.Encode(preserveAlpha ? SKEncodedImageFormat.Png : SKEncodedImageFormat.Jpeg,
                    ((int?) (settings.Quality * 100)) ?? 80);

            var resultStream = new MemoryStream();

            resultData.SaveTo(resultStream);

            resultStream.Seek(0, SeekOrigin.Begin);
            return resultStream;
        }

        static SKRect CalculateDisplayRect(SKRect dest, float bmpWidth, float bmpHeight)
        {
            float x = (dest.Width - bmpWidth) / 2;
            float y = (dest.Height - bmpHeight) / 2;

            x += dest.Left;
            y += dest.Top;

            return new SKRect(x, y, x + bmpWidth, y + bmpHeight);
        }

        private static (float X, float Y) GetScale(int sourceWidth, int sourceHeight, int destWidth, int destHeight, ResizeOption option)
        {
            float sourceAspectRatio = (float) sourceWidth / sourceHeight;
            float destAspectRatio = (float) destWidth / destHeight;

            float scaleX, scaleY;

            switch (option)
            {
                case ResizeOption.AspectFill:
                    scaleX = scaleY = destAspectRatio > sourceAspectRatio
                        ? (float) destWidth / sourceWidth
                        : (float) destHeight / sourceHeight;
                    break;
                case ResizeOption.AspectFit:
                    scaleX = scaleY = destAspectRatio > sourceAspectRatio
                        ? (float) destHeight / sourceHeight
                        : (float) destWidth / sourceWidth;
                    break;
                case ResizeOption.StretchToFill:
                    scaleX = (float) destWidth / sourceWidth;
                    scaleY = (float) destHeight / sourceHeight;
                    break;
                case ResizeOption.None:
                default:
                    scaleX = scaleY = 1.0f;
                    break;
            }

            return (scaleX, scaleY);
        }

        private static (int Width, int Height, Action<SKCanvas> Transform) GetAutoRotateInfo(SKBitmap source, SKEncodedOrigin origin)
        {
            var useWidth = source.Width;
            var useHeight = source.Height;
            Action<SKCanvas> transform = _ => { };

            switch (origin)
            {
                case SKEncodedOrigin.TopLeft:
                    break;
                case SKEncodedOrigin.TopRight:
                    // flip along the x-axis
                    transform = canvas => canvas.Scale(-1, 1, useWidth / 2, useHeight / 2);
                    break;
                case SKEncodedOrigin.BottomRight:
                    transform = canvas => canvas.RotateDegrees(180, useWidth / 2, useHeight / 2);
                    break;
                case SKEncodedOrigin.BottomLeft:
                    // flip along the y-axis
                    transform = canvas => canvas.Scale(1, -1, useWidth / 2, useHeight / 2);
                    break;
                case SKEncodedOrigin.LeftTop:
                    useWidth = source.Height;
                    useHeight = source.Width;
                    transform = canvas =>
                    {
                        // Rotate 90
                        canvas.RotateDegrees(90, useWidth / 2, useHeight / 2);
                        canvas.Scale(useHeight * 1.0f / useWidth, -useWidth * 1.0f / useHeight, useWidth / 2, useHeight / 2);
                    };
                    break;
                case SKEncodedOrigin.RightTop:
                    useWidth = source.Height;
                    useHeight = source.Width;
                    transform = canvas =>
                    {
                        // Rotate 90
                        canvas.RotateDegrees(90, useWidth / 2, useHeight / 2);
                        canvas.Scale(useHeight * 1.0f / useWidth, useWidth * 1.0f / useHeight, useWidth / 2, useHeight / 2);
                    };
                    break;
                case SKEncodedOrigin.RightBottom:
                    useWidth = source.Height;
                    useHeight = source.Width;
                    transform = canvas =>
                    {
                        // Rotate 90
                        canvas.RotateDegrees(90, useWidth / 2, useHeight / 2);
                        canvas.Scale(-useHeight * 1.0f / useWidth, useWidth * 1.0f / useHeight, useWidth / 2, useHeight / 2);
                    };
                    break;
                case SKEncodedOrigin.LeftBottom:
                    useWidth = source.Height;
                    useHeight = source.Width;
                    transform = canvas =>
                    {
                        // Rotate 90
                        canvas.RotateDegrees(90, useWidth / 2, useHeight / 2);
                        canvas.Scale(-useHeight * 1.0f / useWidth, -useWidth * 1.0f / useHeight, useWidth / 2, useHeight / 2);
                    };
                    break;
                default:
                    break;
            }

            return (useWidth, useHeight, transform);
        }
    }
}
