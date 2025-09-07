using HRMS.Application.Common.Interface;
using SkiaSharp;
using ZXing;

namespace HRMS.Infrastructure.Barcode
{
    public class ZXingBarcodeServices : IBarcodeServices
    {
        public byte[] GetBarcodeBytes(string text)
        {
            var writer = new BarcodeWriter<SKBitmap>
            {
                Format = BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 100,
                    Margin = 0
                }
            };

            using var image = writer.Write(text);
            using var ms = new MemoryStream();
            image.Encode(ms, SKEncodedImageFormat.Png, 100);
            return ms.ToArray();
        }
        public string GetBarcodeBase64(string text)
        {
            var bytes = GetBarcodeBytes(text);
            var base64String = Convert.ToBase64String(bytes);
            return base64String;
        }
    }
}
