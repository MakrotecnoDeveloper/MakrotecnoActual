using QRCoder;
using System;

namespace Plataforma.Utils
{
    public static class QrHelper
    {
        public static string ToBase64Png(string payload, int pixelsPerModule = 8)
        {
            payload ??= "";

            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);

            var qr = new PngByteQRCode(data);
            byte[] bytes = qr.GetGraphic(pixelsPerModule);

            return Convert.ToBase64String(bytes);
        }
    }
}