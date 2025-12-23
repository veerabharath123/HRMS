using HRMS.Application.Common.Interface;
using SkiaSharp;

namespace HRMS.Infrastructure.MediaProcessing.ImageCompressor
{

    public sealed class SkiaSharpCompressor : IImageCompressor
    {
        private const int MinJpegQuality = 40;
        private const int MaxJpegQuality = 90;
        private const int ResizeStepPercent = 90; // scale down by 10%

        public byte[] Compress(byte[] inputBytes, long targetSizeInBytes, bool preserveTransparency = true)
        {
            if (inputBytes == null || inputBytes.Length == 0)
                throw new ArgumentException("Input image is empty");

            using var bitmap = DecodeBitmap(inputBytes);

            var hasAlpha = bitmap.AlphaType != SKAlphaType.Opaque;

            // PNG path (only when transparency must be preserved)
            if (hasAlpha && preserveTransparency)
            {
                var png = EncodePng(bitmap);
                if (png.Length <= targetSizeInBytes)
                    return png;
            }

            // JPEG path (with resize + binary search)
            return CompressJpegWithResize(bitmap, targetSizeInBytes);
        }

        // -----------------------
        // Core helpers
        // -----------------------

        private static SKBitmap DecodeBitmap(byte[] input)
        {
            using var stream = new SKMemoryStream(input);
            return SKBitmap.Decode(stream)
                   ?? throw new ArgumentException("Invalid image format");
        }

        private static byte[] EncodePng(SKBitmap bitmap)
        {
            using var ms = new SKDynamicMemoryWStream();
            bitmap.Encode(ms, SKEncodedImageFormat.Png, 100);
            return ms.DetachAsData().ToArray();
        }

        private static byte[] CompressJpegWithResize(SKBitmap original, long targetSize)
        {
            SKBitmap current = original;
            byte[]? best = null;

            try
            {
                while (true)
                {
                    var jpeg = TryCompressJpeg(current, targetSize);
                    if (jpeg != null)
                        return jpeg;

                    best ??= EncodeJpeg(current, MinJpegQuality);

                    // Stop resizing if image is already small
                    if (current.Width < 400 || current.Height < 400)
                        return best;

                    current = ResizeBitmap(current, ResizeStepPercent);
                }
            }
            finally
            {
                if (!ReferenceEquals(current, original))
                    current.Dispose();
            }
        }

        // -----------------------
        // JPEG compression
        // -----------------------

        private static byte[]? TryCompressJpeg(SKBitmap bitmap, long targetSize)
        {
            int low = MinJpegQuality;
            int high = MaxJpegQuality;
            byte[]? candidate = null;

            while (low <= high)
            {
                int mid = (low + high) / 2;
                var data = EncodeJpeg(bitmap, mid);

                if (data.Length <= targetSize)
                {
                    candidate = data;
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }

            return candidate;
        }

        private static byte[] EncodeJpeg(SKBitmap bitmap, int quality)
        {
            using var ms = new SKDynamicMemoryWStream();
            bitmap.Encode(ms, SKEncodedImageFormat.Jpeg, quality);
            return ms.DetachAsData().ToArray();
        }

        // -----------------------
        // Resize
        // -----------------------

        private static SKBitmap ResizeBitmap(SKBitmap source, int percent)
        {
            int width = source.Width * percent / 100;
            int height = source.Height * percent / 100;

            var resized = new SKBitmap(width, height, source.ColorType, source.AlphaType);

            using var canvas = new SKCanvas(resized);
            canvas.DrawBitmap(source, new SKRect(0, 0, width, height));

            return resized;
        }
    }


}
