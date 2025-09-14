using HRMS.Application.Common.Interface;
using SkiaSharp;

namespace HRMS.Infrastructure.ImageCompressor
{
    
    public class SkiaSharpCompressor : IImageCompressor
    {
        public async Task<byte[]> CompressAsync(byte[] inputBytes, long targetSizeInBytes, bool preserveTransparency = true)
        {
            return await Task.Run(() =>
            {
                using var inputStream = new SKMemoryStream(inputBytes);
                using var bitmap = SKBitmap.Decode(inputStream) ?? throw new ArgumentException("Invalid image");

                if (bitmap.ColorType == SKColorType.Unknown)
                    throw new ArgumentException("Unsupported image format");

                return bitmap.ColorType switch
                {
                    SKColorType.Bgra8888 or 
                    SKColorType.Rgba8888 => CompressPng(bitmap, targetSizeInBytes, preserveTransparency),
                    _ => CompressJpeg(bitmap, targetSizeInBytes)
                };
            });
        }

        private static byte[] CompressJpeg(SKBitmap bitmap, long targetSize)
        {
            int low = 10, high = 90, best = 90;
            byte[]? result = null;

            while (low <= high)
            {
                int mid = (low + high) / 2;
                using var ms = new SKDynamicMemoryWStream();
                bitmap.Encode(ms, SKEncodedImageFormat.Jpeg, mid);
                var data = ms.DetachAsData().ToArray();

                if (data.Length <= targetSize) { best = mid; result = data; low = mid + 1; }
                else high = mid - 1;
            }

            return result ?? [];
        }

        private static byte[] CompressPng(SKBitmap bitmap, long targetSize, bool preserveTransparency)
        {
            using var ms = new SKDynamicMemoryWStream();
            bitmap.Encode(ms, SKEncodedImageFormat.Png, 100);
            var data = ms.DetachAsData().ToArray();

            if (data.Length <= targetSize) return data;

            // fallback to JPEG if size too big
            return CompressJpeg(bitmap, targetSize);
        }
    }

}
