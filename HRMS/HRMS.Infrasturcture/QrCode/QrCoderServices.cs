using HRMS.Application.Common.Interface;
using HRMS.Domain.Constants;
using QRCoder;

namespace HRMS.Infrastructure.QrCode
{
    public class QrCoderServices : IQrCodeServices
    {
        public byte[] GetQRBytes(string qrData)
        {
            if (string.IsNullOrEmpty(qrData))
                throw new ArgumentException(GeneralConstants.QR_NO_DATA_MSG, nameof(qrData));

            using QRCodeGenerator qrGenerator = new();
            using QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
            using BitmapByteQRCode qrCode = new(qrCodeData);
            return qrCode.GetGraphic(20);
        }
        public string GetQRBase64(string qrData)
        {
            if (string.IsNullOrEmpty(qrData))
                throw new ArgumentException(GeneralConstants.QR_NO_DATA_MSG, nameof(qrData));

            using QRCodeGenerator qrGenerator = new();
            using QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
            using Base64QRCode qrCode = new(qrCodeData);
            return qrCode.GetGraphic(20);
        }
    }
}
